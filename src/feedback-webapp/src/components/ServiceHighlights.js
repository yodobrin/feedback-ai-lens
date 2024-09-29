import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom'; // Import navigation hook
import './ServiceHighlights.css'; // Import CSS from the same folder

function ServiceHighlights() {
  const [services, setServices] = useState([]);
  const [serviceLogos, setServiceLogos] = useState({}); // Store internal IDs for logos
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    setLoading(true);
    fetch('http://localhost:5229/api/Services/GetServiceHighlights')
      .then((response) => {
        if (!response.ok) {
          throw new Error('Failed to fetch service highlights');
        }
        return response.json();
      })
      .then((data) => {
        setServices(data); // Set the service highlights

        // Fetch internal IDs (logos) for each service
        const fetchServiceLogos = async () => {
          const logos = {};
          for (const service of data) {
            const logo = await getNormalizedServiceName(service.ServiceName);
            logos[service.ServiceName] = logo; // Store the logo with the service name as the key
          }
          setServiceLogos(logos); // Update state with the fetched logos
        };

        fetchServiceLogos();
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

  const getNormalizedServiceName = async (serviceName) => {
    try {
      const response = await fetch(`http://localhost:5229/api/Services/GetInternalId?serviceName=${encodeURIComponent(serviceName)}`);
      if (!response.ok) {
        throw new Error('Failed to fetch internalId');
      }
      const data = await response.json();
      return data.internalId;
    } catch (error) {
      console.error("Error fetching internalId:", error);
      return 'NO_SUCH_SERVICE'; // Fallback for unknown services
    }
  };

  return (
    <div className="service-highlights">
      <h2>Product Leaders Copilot: AI-Driven Insights from Customer Feedback</h2>
      {loading && <div className="loading-indicator">Loading services...</div>}
      {error && <div className="error-message">{error}</div>}

      {!loading && !error && services.length > 0 && (
        <div className="cards-container">
          {services.map((service, index) => (
            <div
              className="card"
              key={index}
              onClick={() => handleServiceClick(service.ServiceName)}
            >
              <h3>{service.ServiceName}</h3>
              <p><strong>Total Feedback:</strong> {service.TotalFeedback}</p>
              <p><strong>Distinct Customers:</strong> {service.DistinctCustomers}</p>

              <hr className="divider" />

              <p><strong>Feedback Types:</strong></p>
              <div className="feedback-types">
                {service.feedbackTypes.map((feedbackType, i) => (
                  <div key={i} className="feedback-type">
                    <strong>{feedbackType.type}:</strong> {feedbackType.count}
                    {feedbackType.details.length > 0 && (
                      <div className="feedback-details">
                        {feedbackType.details.map((detail, j) => (
                          <p key={j} style={{ marginLeft: '15px', textAlign: 'left' }}>
                            <em>{detail.originalType.trim() !== '' ? detail.originalType : 'No Category'}:</em> {detail.count}
                          </p>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>

              {/* Dynamically load image based on the service name */}
              <img
                src={`${process.env.PUBLIC_URL}/${serviceLogos[service.ServiceName] || 'NO_SUCH_SERVICE'}_image.png`}
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
