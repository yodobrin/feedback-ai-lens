#!/usr/bin/env python3

import json
import argparse
from collections import defaultdict
import numpy as np
from sklearn.cluster import DBSCAN

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--input_file", required=True, help="Path to the JSON with FeedbackRecords")
    parser.add_argument("--output_file", default="clustered_records.json", help="Where to save results")
    parser.add_argument("--eps", type=float, default=1.2, help="DBSCAN eps parameter")
    parser.add_argument("--min_samples", type=int, default=4, help="DBSCAN min_samples parameter")
    args = parser.parse_args()

    # 1. Load the feedback records from JSON
    with open(args.input_file, 'r', encoding='utf-8') as f:
        feedback_records = json.load(f)

    # 2. Extract embeddings into a 2D numpy array
    #    and store them in X for DBSCAN
    embeddings = [rec["Embedding"] for rec in feedback_records]
    X = np.array(embeddings, dtype=float)

    # 3. Perform DBSCAN clustering
    dbscan = DBSCAN(eps=args.eps, min_samples=args.min_samples, metric="euclidean")
    labels = dbscan.fit_predict(X)  # array of cluster labels, -1 means outlier

    # 4. Attach cluster label, remove "Embedding", and gather cluster summaries
    cluster_summary = defaultdict(lambda: {"count": 0, "customers": set()})

    for i, rec in enumerate(feedback_records):
        cluster_id = int(labels[i])
        rec["Cluster"] = cluster_id

        # Remove the Embedding field to shrink size
        rec.pop("Embedding", None)

        # Update summary info
        cluster_summary[cluster_id]["count"] += 1
        cluster_summary[cluster_id]["customers"].add(rec["CustomerName"])

    # 5. Print cluster summaries
    # Sort cluster IDs so we print them in a stable order (esp. if -1 for outliers)
    sorted_cluster_ids = sorted(cluster_summary.keys())
    for cid in sorted_cluster_ids:
        info = cluster_summary[cid]
        unique_customers = len(info["customers"])
        print(f"Cluster {cid}: {info['count']} items, {unique_customers} unique customers")

    # 6. Write updated records (w/o embeddings) to the output file
    with open(args.output_file, 'w', encoding='utf-8') as outf:
        json.dump(feedback_records, outf, indent=2)

    print(f"\nWrote {len(feedback_records)} records to '{args.output_file}'. Embeddings removed.")

if __name__ == "__main__":
    main()
