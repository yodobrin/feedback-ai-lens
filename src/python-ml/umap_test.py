#!/usr/bin/env python3
import json
import numpy as np
from sklearn.cluster import DBSCAN
import umap

# Example function to show how you'd reduce dimensions with UMAP, then cluster
def cluster_with_umap(feedback_records,
                      n_neighbors=18,
                      min_dist=0.15,
                      n_components=50,
                      metric='cosine',
                      dbscan_eps=0.75,
                      dbscan_min_samples=8):

    # 1) Convert embeddings to a numpy array
    X = np.array([rec["Embedding"] for rec in feedback_records], dtype=float)

    # 2) UMAP dimension reduction
    reducer = umap.UMAP(
        n_neighbors=n_neighbors,
        min_dist=min_dist,
        n_components=n_components,
        metric=metric,
        random_state=32
    )
    X_reduced = reducer.fit_transform(X)  # shape: (N, n_components)

    # 3) DBSCAN in the reduced space
    dbscan = DBSCAN(eps=dbscan_eps, min_samples=dbscan_min_samples, metric='cosine')
    labels = dbscan.fit_predict(X_reduced)

    # Summaries
    n_outliers = (labels == -1).sum()
    cluster_ids = set(labels) - {-1}
    print(f"UMAP -> DBSCAN: #clusters={len(cluster_ids)}, #outliers={n_outliers}")
    return labels

if __name__ == "__main__":
    # Example usage
    with open("../../sample-data/adf.json", "r") as f:
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
