import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ThemeAnalysis from './ThemeAnalysis';  // Import the ThemeAnalysis component
import IssueAnalysis from './IssueAnalysis';  // Import the IssueAnalysis component
import './ServiceClusters.css';  // Ensure the correct CSS import

function ServiceClusters() {
  const { serviceName } = useParams();
  const [clusters, setClusters] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    // Fetch clusters data for the selected service
    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${serviceName}`)
      .then(response => response.json())
      .then(data => setClusters(data))
      .catch(error => console.error('Error fetching clusters:', error));
  }, [serviceName]);

  const goBack = () => {
    navigate('/');  // Navigate back to the main page
  };

  return (
    <div className="service-clusters">
      <h3>{serviceName} Clusters</h3>
      <div className="cluster-list">
        {clusters.length > 0 ? (
          clusters.map((cluster, index) => (
            <div key={index} className="cluster-card">
              <h4>{cluster.ClusterName}</h4>
              <p><strong>Common Element:</strong> {cluster.CommonElement}</p>
              <p><strong>Similar Feedbacks:</strong> {cluster.SimilarFeedbacks}</p>
              <p><strong>Distinct Customers:</strong> {cluster.DistinctCustomers}</p>
            </div>
          ))
        ) : (
          <p>No clusters available.</p>
        )}
      </div>
      <button onClick={goBack} className="back-button">Back to Main Page</button>

      {/* Embedding ThemeAnalysis and IssueAnalysis Components */}
      <ThemeAnalysis serviceName={serviceName} />
      <IssueAnalysis serviceName={serviceName} />
    </div>
  );
}

export default ServiceClusters;