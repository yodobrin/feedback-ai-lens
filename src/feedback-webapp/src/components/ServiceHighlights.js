import React from 'react';

function ServiceHighlights({ serviceHighlights, onServiceClick }) {
  return (
    <div>
      <h1>Azure Service Highlights</h1>
      <div className="cards-container">
        {serviceHighlights.map((highlight, index) => (
          <div className="card" key={index} onClick={() => onServiceClick(highlight.ServiceName)}>
            <h3>{highlight.ServiceName}</h3>
            <p>Total Feedback: {highlight.TotalFeedback}</p>
            <p>Distinct Customers: {highlight.DistinctCustomers}</p>
            <p>Feature Requests: {highlight.FeatureRequests}</p>
            <p>Bugs: {highlight.Bugs}</p>
            <p>Overall Sentiment: {highlight.OverallSentiment}</p>
          </div>
        ))}
      </div>
    </div>
  );
}

export default ServiceHighlights;