#!/usr/bin/env python3

"""
Example usage:
  python multi_clustering.py --input_file feedback_records.json

What it does:
1) Loads the input JSON of feedback records (each with an "Embedding").
2) Runs DBSCAN, HDBSCAN, AgglomerativeClustering, and K-Means with various parameter combos.
3) For each run, computes:
   - n_clusters (excluding outliers if applicable)
   - n_outliers (DBSCAN/HDBSCAN only)
   - n_customers_union (union of unique customers across all clusters)
   - avg_customers_per_cluster (average # unique customers per cluster)
4) Prints a summary DataFrame. No extra JSON files are created.

Note on Agglomerative:
- This script uses AgglomerativeClustering(..., metric='euclidean', ...),
  which requires scikit-learn >= 1.2. In older versions, you may need
  affinity='euclidean' instead of metric='euclidean'.
"""

import argparse
import json
import numpy as np
import pandas as pd

from sklearn.cluster import DBSCAN, AgglomerativeClustering, KMeans
import hdbscan

import umap

# Example function to show how you'd reduce dimensions with UMAP, then cluster
def cluster_with_umap(feedback_records,
                      n_neighbors=15,
                      min_dist=0.1,
                      n_components=50,
                      metric='cosine',
                      dbscan_eps=0.3,
                      dbscan_min_samples=5):

    # 1) Convert embeddings to a numpy array
    X = np.array([rec["Embedding"] for rec in feedback_records], dtype=float)

    # 2) UMAP dimension reduction
    reducer = umap.UMAP(
        n_neighbors=n_neighbors,
        min_dist=min_dist,
        n_components=n_components,
        metric=metric,
        random_state=42
    )
    X_reduced = reducer.fit_transform(X)  # shape: (N, n_components)

    # 3) DBSCAN in the reduced space
    dbscan = DBSCAN(eps=dbscan_eps, min_samples=dbscan_min_samples, metric='euclidean')
    labels = dbscan.fit_predict(X_reduced)

    # Summaries
    n_outliers = (labels == -1).sum()
    cluster_ids = set(labels) - {-1}
    print(f"UMAP -> DBSCAN: #clusters={len(cluster_ids)}, #outliers={n_outliers}")
    return labels

if __name__ == "__main__":
    # Example usage
    with open("feedback_records.json", "r") as f:
        records = json.load(f)

    labels = cluster_with_umap(
        feedback_records=records,
        n_neighbors=15,         # tune
        min_dist=0.1,           # tune
        n_components=50,        # tune dimension
        metric='cosine',        # UM

        # DBSCAN parameters
        dbscan_eps=0.3,
        dbscan_min_samples=5
    )
def cluster_summary(labels, feedback_records):
    """
    Given an array of cluster labels (any integer, -1 for outliers),
    returns:
      - n_clusters (excluding -1)
      - n_outliers
      - n_customers_union: # of unique customers across all non-outlier clusters
      - avg_customers_per_cluster: average # of unique customers *within* each cluster.
    """

    # Identify the set of cluster IDs
    cluster_ids = set(labels)
    # Exclude -1 from "valid" cluster IDs if it exists
    non_outlier_ids = cluster_ids - {-1} if -1 in cluster_ids else cluster_ids

    n_clusters = len(non_outlier_ids)
    n_outliers = (labels == -1).sum() if -1 in cluster_ids else 0

    # Build a dict: cluster_id -> set_of_customers
    cluster_to_customers = {}
    for cid in non_outlier_ids:
        cluster_to_customers[cid] = set()

    # Assign each record's customer to the set for that cluster (if not outlier)
    for i, cid in enumerate(labels):
        if cid in non_outlier_ids:
            cust = feedback_records[i]["CustomerName"]
            cluster_to_customers[cid].add(cust)

    if n_clusters == 0:
        # Edge case: everything might be outlier, or no clusters at all.
        return {
            "n_clusters": 0,
            "n_outliers": n_outliers,
            "n_customers_union": 0,
            "avg_customers_per_cluster": 0.0
        }

    # 1) "Union" of unique customers across all clusters
    union_of_custs = set()
    for cid in non_outlier_ids:
        union_of_custs.update(cluster_to_customers[cid])
    n_customers_union = len(union_of_custs)

    # 2) "Average unique customers per cluster"
    per_cluster_counts = [len(cluster_to_customers[cid]) for cid in non_outlier_ids]
    avg_customers_per_cluster = float(np.mean(per_cluster_counts))  # or median, etc.

    return {
        "n_clusters": n_clusters,
        "n_outliers": n_outliers,
        "n_customers_union": n_customers_union,
        "avg_customers_per_cluster": round(avg_customers_per_cluster, 2)
    }


def run_dbscan(X, feedback_records, eps, min_samples):
    dbscan = DBSCAN(eps=eps, min_samples=min_samples, metric='euclidean')
    labels = dbscan.fit_predict(X)
    stats = cluster_summary(labels, feedback_records)
    return {
        "Algorithm": "DBSCAN",
        "Params": f"eps={eps}, min_samp={min_samples}",
        **stats
    }


def run_hdbscan(X, feedback_records, min_cluster_size, min_samples, cluster_selection_epsilon):
    # If your hdbscan supports 'cosine', you can do metric='cosine'
    clusterer = hdbscan.HDBSCAN(
        metric='euclidean',
        min_cluster_size=min_cluster_size,
        min_samples=min_samples,
        cluster_selection_epsilon=cluster_selection_epsilon,
        cluster_selection_method='eom'
    )
    labels = clusterer.fit_predict(X)
    stats = cluster_summary(labels, feedback_records)
    return {
        "Algorithm": "HDBSCAN",
        "Params": (f"min_cluster_size={min_cluster_size}, "
                   f"min_samples={min_samples}, "
                   f"eps={cluster_selection_epsilon}"),
        **stats
    }


def run_agglomerative(X, feedback_records, n_clusters, linkage):
    # For scikit-learn >= 1.2: metric='euclidean'
    # If older version, you might need: affinity='euclidean'
    agg = AgglomerativeClustering(
        n_clusters=n_clusters,
        metric='euclidean',
        linkage=linkage
    )
    labels = agg.fit_predict(X)
    stats = cluster_summary(labels, feedback_records)  # No outliers for Agglo
    return {
        "Algorithm": "Agglomerative",
        "Params": f"n_clust={n_clusters}, link={linkage}",
        **stats
    }


def run_kmeans(X, feedback_records, n_clusters):
    kmeans = KMeans(n_clusters=n_clusters, random_state=0)
    labels = kmeans.fit_predict(X)
    # K-Means has no outliers, but we reuse same summary
    stats = cluster_summary(labels, feedback_records)
    return {
        "Algorithm": "K-Means",
        "Params": f"n_clusters={n_clusters}",
        **stats
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--input_file", required=True, help="Path to feedback_records JSON.")
    args = parser.parse_args()

    # 1) Load data
    with open(args.input_file, "r", encoding="utf-8") as f:
        feedback_records = json.load(f)

    # 2) Convert embeddings to numpy
    embeddings = [rec["Embedding"] for rec in feedback_records]
    X = np.array(embeddings, dtype=float)

    results = []

    # 3) Define parameter grids
    dbscan_params = [
        {"eps": 0.2, "min_samples": 3},
        {"eps": 0.3, "min_samples": 5},
        {"eps": 0.35, "min_samples": 10},
        {"eps": 0.4, "min_samples": 10},
        {"eps": 0.4, "min_samples": 5},
        {"eps": 0.45, "min_samples": 5},
    ]
    hdbscan_params = [
        {"min_cluster_size": 5, "min_samples": 5, "cluster_selection_epsilon": 0.0},
        {"min_cluster_size": 10, "min_samples": 10, "cluster_selection_epsilon": 0.2},
        {"min_cluster_size": 10, "min_samples": 5, "cluster_selection_epsilon": 0.2},
        {"min_cluster_size": 5, "min_samples": 5, "cluster_selection_epsilon": 0.2},
        {"min_cluster_size": 5, "min_samples": 5, "cluster_selection_epsilon": 0.3},
        {"min_cluster_size": 5, "min_samples": 5, "cluster_selection_epsilon": 0.4},
    ]
    agg_params = [
        {"n_clusters": 50,  "linkage": "average"},
        {"n_clusters": 50, "linkage": "complete"}
    ]
    kmeans_params = [
        {"n_clusters": 10},
        {"n_clusters": 50}
    ]

    # 4) Run each algorithm with each param combo

    # DBSCAN
    for p in dbscan_params:
        row = run_dbscan(X, feedback_records, p["eps"], p["min_samples"])
        results.append(row)

    # HDBSCAN
    for p in hdbscan_params:
        row = run_hdbscan(
            X, feedback_records,
            p["min_cluster_size"], p["min_samples"],
            p["cluster_selection_epsilon"]
        )
        results.append(row)

    # Agglomerative
    for p in agg_params:
        row = run_agglomerative(X, feedback_records, p["n_clusters"], p["linkage"])
        results.append(row)

    # K-Means
    for p in kmeans_params:
        row = run_kmeans(X, feedback_records, p["n_clusters"])
        results.append(row)

    # 5) Convert results to DataFrame & print
    df = pd.DataFrame(results)
    print("\n===== CLUSTERING SUMMARY =====")
    print(df.to_string(index=False))
    print("================================\n")


if __name__ == "__main__":
    main()

