import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import './ServiceClusters.css';

function ServiceClusters() {
  const { serviceName } = useParams();
  const [clusters, setClusters] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0); // State to track the current carousel index
  const navigate = useNavigate();

  useEffect(() => {
    // Fetch clusters data for the selected service
    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${serviceName}`)
      .then(response => response.json())
      .then(data => setClusters(data))
      .catch(error => console.error('Error fetching clusters:', error));
  }, [serviceName]);

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

  // Navigate to the deep dive view for the selected cluster
  const goToDeepDive = (cluster) => {
    navigate(`/cluster-deep-dive`, { state: { cluster, serviceName } });
  };

  const goBack = () => {
    navigate('/');  // Navigate back to the main page
  };

  const goToThemeAnalysis = () => {
    navigate(`/theme-analysis`, { state: { serviceName } }); // Navigate to Theme Analysis
  };

  const goToIssueAnalysis = () => {
    navigate(`/issue-analysis`, { state: { serviceName } }); // Navigate to Issue Analysis
  };
  const goToHome = () => {
    navigate('/'); // Navigate to the home page
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
    <div className="service-clusters">
    {/* Home icon at the top right */}
    <div className="home-container">
      <img
        src="/home.png"
        alt="Home"
        className="home-icon"
        onClick={goToHome}  // Navigate to the home page
        style={{ cursor: 'pointer' }}
      />
    </div>
    {/* Cluster header with service logo and name in one line */}
    <div className="cluster-header">
      <img
        src={`${process.env.PUBLIC_URL}/${getNormalizedServiceName(serviceName)}_image.png`}
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
                <h4>{cluster.CommonElement}</h4><br></br>
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
        onClick={goToHome}  // Add navigation to the home page when the logo is clicked
        style={{ cursor: 'pointer' }}  // Make the logo clickable
      />
    </div>
  );
}

export default ServiceClusters;
