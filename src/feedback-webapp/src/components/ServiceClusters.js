import React, { useState, useEffect } from 'react';
import ThemeAnalysis from './ThemeAnalysis';  // Handles issue analysis
import IssueAnalysis from './IssueAnalysis';  // Future feature for customer search

function ServiceClusters({ selectedService, onBack }) {
  const [serviceClusters, setServiceClusters] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${selectedService}`)
      .then(response => response.json())
      .then(data => setServiceClusters(data))
      .catch(error => {
        console.error('Error fetching clusters:', error);
        setError('Failed to load clusters');
      });
  }, [selectedService]);

  return (
    <div>
      <button onClick={onBack}>Back to Highlights</button>
      <h2>{selectedService} Clusters</h2>

      {error && <div className="error-message">{error}</div>}

      <div className="cards-container">
        {serviceClusters.map((cluster, index) => (
          <div className="cluster-card" key={index}>
            <h3>{cluster.ClusterName}</h3>
            <p>Common Element: {cluster.CommonElement}</p>
            <p>Similar Feedbacks: {cluster.SimilarFeedbacks}</p>
            <p>Distinct Customers: {cluster.DistinctCustomers}</p>
          </div>
        ))}
      </div>

      {/* Analyze Section */}
      <ThemeAnalysis serviceName={selectedService} />

      {/* Future Feature: Search for impacted customers by issue or theme */}
      <IssueAnalysis serviceName={selectedService} />
    </div>
  );
}

export default ServiceClusters;