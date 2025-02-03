#!/usr/bin/env python3

"""
Example usage:
  python hdbscan_script.py \
    --input_file feedback_records.json \
    --output_file hdbscan_clusters.json \
    --metric cosine \
    --min_cluster_size 5 \
    --min_samples 5 \
    --cluster_selection_epsilon 0.0 \
    --cluster_selection_method eom

After running, it will:
1) Load the JSON of records (each with an "Embedding" array).
2) Perform HDBSCAN clustering.
3) Assign a "Cluster" ID to each record (or -1 if outlier).
4) Remove the "Embedding" to reduce size.
5) Print summary of each cluster (# items, # unique customers).
6) Save updated records to --output_file as JSON.
"""

import json
import argparse
from collections import defaultdict
import numpy as np
import hdbscan

def main():
    parser = argparse.ArgumentParser(description="Cluster feedback records using HDBSCAN.")
    parser.add_argument("--input_file", required=True, help="Path to the JSON with FeedbackRecords.")
    parser.add_argument("--output_file", default="hdbscan_clusters.json", help="Where to save results.")
    parser.add_argument("--metric", default="euclidean", help="Distance metric: e.g. 'cosine' or 'euclidean'.")
    parser.add_argument("--min_cluster_size", type=int, default=5,
                        help="Minimum cluster size for HDBSCAN.")
    parser.add_argument("--min_samples", type=int, default=5,
                        help="Min samples for a dense region; controls outlier vs core distribution.")
    parser.add_argument("--cluster_selection_epsilon", type=float, default=0.6,
                        help="Cluster selection epsilon (helps refine final clusters).")
    parser.add_argument("--cluster_selection_method", default="eom",
                        choices=["eom", "leaf"],
                        help="Method for extracting clusters from the hierarchical tree.")
    args = parser.parse_args()

    # 1. Load the feedback records from JSON
    with open(args.input_file, "r", encoding="utf-8") as f:
        feedback_records = json.load(f)

    # 2. Extract embeddings into a 2D numpy array
    embeddings = [rec["Embedding"] for rec in feedback_records]
    X = np.array(embeddings, dtype=float)

    # 3. Set up HDBSCAN
    clusterer = hdbscan.HDBSCAN(
        metric=args.metric,
        min_cluster_size=args.min_cluster_size,
        min_samples=args.min_samples,
        cluster_selection_epsilon=args.cluster_selection_epsilon,
        cluster_selection_method=args.cluster_selection_method
    )

    labels = clusterer.fit_predict(X)  # array of cluster IDs, -1 for outliers

    # 4. Build a summary of clusters; remove 'Embedding'
    cluster_summary = defaultdict(lambda: {"count": 0, "customers": set()})

    for i, rec in enumerate(feedback_records):
        cid = int(labels[i])  # cluster label (could be -1)
        rec["Cluster"] = cid

        # Remove the embedding to shrink size
        rec.pop("Embedding", None)

        cluster_summary[cid]["count"] += 1
        cluster_summary[cid]["customers"].add(rec["CustomerName"])

    # 5. Print cluster summaries
    sorted_ids = sorted(cluster_summary.keys())
    for cid in sorted_ids:
        info = cluster_summary[cid]
        unique_cust = len(info["customers"])
        print(f"Cluster {cid}: {info['count']} items, {unique_cust} unique customers")

    # 6. Save updated records (minus embeddings) to output file
    with open(args.output_file, "w", encoding="utf-8") as outf:
        json.dump(feedback_records, outf, indent=2)

    print(f"\nWrote {len(feedback_records)} records to '{args.output_file}'. "
          f"Used HDBSCAN with metric='{args.metric}' and min_cluster_size={args.min_cluster_size}.")

if __name__ == "__main__":
    main()
