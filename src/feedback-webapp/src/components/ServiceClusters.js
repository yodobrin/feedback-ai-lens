import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './ServiceClusters.css';

function ServiceClusters() {
  const { serviceName } = useParams();
  const [clusters, setClusters] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [serviceLogo, setServiceLogo] = useState('NO_SUCH_SERVICE'); // State to hold service logo
  const navigate = useNavigate();

  useEffect(() => {
    // Fetch service internalId (serviceLogo)
    const fetchServiceLogo = async () => {
      const logo = await getNormalizedServiceName(serviceName);
      setServiceLogo(logo); // Set the logo after fetching the internalId
    };

    fetchServiceLogo();

    // Fetch clusters data for the selected service
    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${serviceName}`)
      .then(response => response.json())
      .then(data => setClusters(data))
      .catch(error => console.error('Error fetching clusters:', error));
  }, [serviceName]);

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

  const nextCluster = () => {
    if (currentIndex < clusters.length - 3) {
      setCurrentIndex(currentIndex + 1);
    }
  };

  const prevCluster = () => {
    if (currentIndex > 0) {
      setCurrentIndex(currentIndex - 1);
    }
  };

  const goToDeepDive = (cluster) => {
    navigate(`/cluster-deep-dive`, { state: { cluster, serviceName } });
  };

  const goBack = () => {
    navigate('/'); // Navigate back to the main page
  };

  const goToHome = () => {
    navigate('/'); // Navigate to the home page
  };

  return (
    <div className="service-clusters">
      {/* Home icon at the top right */}
      <div className="home-container">
        <img
          src="/home.png"
          alt="Home"
          className="home-icon"
          onClick={goToHome}
          style={{ cursor: 'pointer' }}
        />
      </div>

      {/* Cluster header with service logo and name in one line */}
      <div className="cluster-header">
        <img
          src={`${process.env.PUBLIC_URL}/${serviceLogo}_image.png`}
          alt={`${serviceName} Logo`}
          className="cluster-service-image"
        />
        <h2>{serviceName} - Main Feedback Clusters</h2>
      </div>

      <div className="cluster-carousel">
        {clusters.length > 0 ? (
          <div className="cluster-list">
            {clusters.slice(currentIndex, currentIndex + 3).map((cluster, index) => (
              <div key={index} className="cluster-card">
                <h4>{cluster.CommonElement}</h4>
                <div className="summary">
                  <strong>Summary:</strong>
                  <p>{`${cluster.Summary.slice(0, 300)}...`}</p>
                </div>
                <p><strong>Similar Feedbacks:</strong> {cluster.SimilarFeedbacks}</p>
                <p><strong>Distinct Customers:</strong> {cluster.DistinctCustomers}</p>
                <button onClick={() => goToDeepDive(cluster)} className="deep-dive-button">Deep Dive</button>
              </div>
            ))}
          </div>
        ) : (
          <p>No clusters available.</p>
        )}
      </div>

      <div className="carousel-controls">
        <button onClick={prevCluster} className="carousel-button" disabled={currentIndex === 0}>Previous</button>
        <button onClick={nextCluster} className="carousel-button" disabled={currentIndex >= clusters.length - 3}>Next</button>
      </div>

      {/* Additional buttons for navigating to Theme Analysis and Issue Analysis */}
      <div className="additional-navigation">
        <button onClick={goToThemeAnalysis} className="theme-analysis-button">Feedback Search</button>
        <button onClick={goToIssueAnalysis} className="issue-analysis-button">Customer Search</button>
      </div>

      {/* <button onClick={goBack} className="back-button">Back to Main Page</button> */}
      {/* Clickable logo to navigate to the home view */}
      <img
        src="/hacklogo.png"
        alt="Logo"
        className="bottom-right-logo"
        onClick={goToHome}
        style={{ cursor: 'pointer' }}
      />
    </div>
  );
}

export default ServiceClusters;
