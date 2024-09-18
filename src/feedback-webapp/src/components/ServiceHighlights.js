import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; // Import navigation hook
import './ServiceHighlights.css'; // Import CSS from the same folder

function ServiceHighlights() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    setLoading(true);
    fetch('http://localhost:5229/api/Services/GetServiceHighlights') // Adjust the API URL
      .then((response) => {
        if (!response.ok) {
          throw new Error('Failed to fetch service highlights');
        }
        return response.json();
      })
      .then((data) => {
        setServices(data); // Set the service highlights
        setError(null);
      })
      .catch((err) => {
        setError('Failed to load service highlights');
        console.error(err);
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  const handleServiceClick = (serviceName) => {
    navigate(`/service-clusters/${serviceName}`); // Navigate to the ServiceClusters page with serviceName
  };
  const getNormalizedServiceName = (serviceName) => {
    // Check for specific keywords and return a custom image name
    if (serviceName.toLowerCase().includes('factory')) {
      return 'azure_data_factory';
    } else if (serviceName.toLowerCase().includes('cosmos')) {
      return 'cosmos_db';
    } else if (serviceName.toLowerCase().includes('kuber')) {
      return 'azure_kubernetes';
    }
  }

  return (
    <div className="service-highlights">
      <br></br><h2>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FeedbackLens: AI-Driven Insights for Customer Feedback</h2><br></br><br></br>
      {loading && <div className="loading-indicator">Loading services...</div>}
      {error && <div className="error-message">{error}</div>}

      {!loading && !error && services.length > 0 && (
        <div className="cards-container">
          {services.map((service, index) => (
            <div
              className="card"
              key={index}
              onClick={() => handleServiceClick(service.ServiceName)} // Pass the serviceName to the handle click function
            >
              <h3>{service.ServiceName}</h3><br></br>
              <p align="left"><strong >Total Feedback:</strong> {service.TotalFeedback}</p>
              <p align="left"><strong>Distinct Customers:</strong> {service.DistinctCustomers}</p>
              <p align="left"><strong>Feature Requests:</strong> {service.FeatureRequests}</p>
              <p align="left"><strong>Bugs:</strong> {service.Bugs}</p>
              <p align="left"><strong>Overall Sentiment:</strong> {service.OverallSentiment}</p>
              {/* Dynamically load image based on the service name */}
              <img
                src={`${process.env.PUBLIC_URL}/${getNormalizedServiceName(service.ServiceName)}_image.png`}
                alt={`${service.ServiceName} Logo`}
                className="service-image"
              />

            </div>
          ))}
        </div>
      )}
      <img src="/hacklogo.png" alt="Logo" className="bottom-right-logo" />
    </div>
  );
}

export default ServiceHighlights;
