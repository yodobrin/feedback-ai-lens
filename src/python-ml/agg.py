#!/usr/bin/env python3

import json
import argparse
from collections import defaultdict
import numpy as np
from sklearn.cluster import AgglomerativeClustering
from sklearn.metrics.pairwise import cosine_distances

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--input_file", required=True)
    parser.add_argument("--output_file", default="hc_clusters.json")
    parser.add_argument("--n_clusters", type=int, default=50, help="Number of clusters to form")
    parser.add_argument("--linkage", default="average", choices=["ward","complete","average","single"],
                        help="Linkage criterion for hierarchical clustering")
    args = parser.parse_args()

    # 1. Load feedback records
    with open(args.input_file, "r", encoding="utf-8") as f:
        feedback_records = json.load(f)

    # 2. Extract embeddings -> NxD matrix
    embeddings = [rec["Embedding"] for rec in feedback_records]
    X = np.array(embeddings, dtype=float)

    # 3. Compute a distance matrix using cosine distance
    #    shape: (N, N)
    dist_matrix = cosine_distances(X)

    # 4. Hierarchical clustering with precomputed distance
    #    Note: 'affinity="precomputed"' is for distances, so we can't use 'ward' here
    #    Ward requires Euclidean spaces. So we set 'linkage' to average/complete/single
    hc = AgglomerativeClustering(
        n_clusters=args.n_clusters,
        linkage=args.linkage
    )

    labels = hc.fit_predict(dist_matrix)  # returns cluster labels 0..(n_clusters-1)

    # 5. Attach cluster, remove embedding, build summary
    cluster_summary = defaultdict(lambda: {"count": 0, "customers": set()})

    for i, rec in enumerate(feedback_records):
        cid = int(labels[i])
        rec["Cluster"] = cid
        rec.pop("Embedding", None)
        cluster_summary[cid]["count"] += 1
        cluster_summary[cid]["customers"].add(rec["CustomerName"])

    # 6. Print cluster summaries
    sorted_cluster_ids = sorted(cluster_summary.keys())
    for cid in sorted_cluster_ids:
        info = cluster_summary[cid]
        print(f"Cluster {cid}: {info['count']} items, {len(info['customers'])} unique customers")

    # 7. Write updated records
    with open(args.output_file, "w", encoding="utf-8") as outf:
        json.dump(feedback_records, outf, indent=2)

    print(f"\nWrote {len(feedback_records)} records to '{args.output_file}'. Embeddings removed.")

if __name__ == "__main__":
    main()
