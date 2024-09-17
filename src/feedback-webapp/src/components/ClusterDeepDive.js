import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './ClusterDeepDive.css';  // Create a CSS file for this component

function ClusterDeepDive() {
  const navigate = useNavigate();
  const location = useLocation();
  const { cluster, serviceName } = location.state || {};

  if (!cluster) {
    navigate('/'); // Redirect if no cluster is passed
    return null;
  }

  const goBack = () => {
    navigate(-1); // Go back to the previous page
  };

  return (
    <div className="cluster-deep-dive">
      <h2>Deep Dive into Cluster: {cluster.CommonElement} [{serviceName}]</h2>

      <div className="summary">
        <strong>Summary:</strong>
        <p>{cluster.Summary}</p>
      </div>

      <h4>Feedback Items</h4>
      <ul>
        {cluster.FeedbackRecords.map((feedback, index) => (
          <li key={index} className="feedback-item">
            <strong>Title:</strong> {feedback.Title}<br />
            <strong>Customer:</strong> {feedback.CustomerName}<br />
            <strong>User Story:</strong> <em>{feedback.UserStory}</em>
          </li>
        ))}
      </ul>

      <button onClick={goBack} className="back-button">Back to Clusters</button>
    </div>
  );
}

export default ClusterDeepDive;
