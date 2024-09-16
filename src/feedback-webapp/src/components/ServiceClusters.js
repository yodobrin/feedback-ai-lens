import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import ThemeAnalysis from './ThemeAnalysis';  // Import the ThemeAnalysis component
import IssueAnalysis from './IssueAnalysis';  // Import the IssueAnalysis component
import './ServiceClusters.css';  // Ensure the correct CSS import

function ServiceClusters() {
  const { serviceName } = useParams();
  const [clusters, setClusters] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0); // State to track the current carousel index
  const [selectedClusterId, setSelectedClusterId] = useState(null); // Track selected cluster ID
  const [feedbackIndex, setFeedbackIndex] = useState(0); // Track feedback pagination per cluster
  const [expanded, setExpanded] = useState({});  // State for toggling summary for each cluster
  const [feedbackExpanded, setFeedbackExpanded] = useState({});  // State for feedback toggling
  const navigate = useNavigate();

  useEffect(() => {
    // Fetch clusters data for the selected service
    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${serviceName}`)
      .then(response => response.json())
      .then(data => {
        setClusters(data);

        // Set the initial state of `expanded` to make all summaries expanded by default
        const initialExpandedState = {};
        data.forEach(cluster => {
          initialExpandedState[cluster.ClusterId] = true;
        });
        setExpanded(initialExpandedState);

        // Set the initial feedback state to not expanded for any cluster
        const initialFeedbackState = {};
        data.forEach(cluster => {
          initialFeedbackState[cluster.ClusterId] = false;
        });
        setFeedbackExpanded(initialFeedbackState);
      })
      .catch(error => console.error('Error fetching clusters:', error));
  }, [serviceName]);

  const goBack = () => {
    navigate('/');  // Navigate back to the main page
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

  const handleClusterClick = (clusterId) => {
    // If the clicked cluster is already selected, deselect it
    setSelectedClusterId(selectedClusterId === clusterId ? null : clusterId);
    setFeedbackIndex(0); // Reset feedback pagination on cluster selection
  };

  // Feedback pagination
  const nextFeedback = (feedbackLength) => {
    if (feedbackIndex < feedbackLength - 3) {
      setFeedbackIndex(feedbackIndex + 1);
    }
  };

  const prevFeedback = () => {
    if (feedbackIndex > 0) {
      setFeedbackIndex(feedbackIndex - 1);
    }
  };

  // Function to toggle summary for each cluster
  const toggleSummary = (clusterId) => {
    setExpanded((prevState) => ({
      ...prevState,
      [clusterId]: !prevState[clusterId],  // Toggle for the specific cluster
    }));
  };

  // Function to toggle feedback for each cluster
  const toggleFeedback = (clusterId) => {
    setFeedbackExpanded((prevState) => ({
      ...prevState,
      [clusterId]: !prevState[clusterId],  // Toggle feedback expansion
    }));
  };

  return (
    <div className="service-clusters">
      <h3>{serviceName} Clusters</h3>

      <div className="cluster-carousel">
        {clusters.length > 0 ? (
          <div className="cluster-list">
            {clusters.slice(currentIndex, currentIndex + 3).map((cluster, index) => (
              <div
                key={index}
                className={`cluster-card ${selectedClusterId === cluster.ClusterId ? 'selected' : ''}`}
                onClick={() => handleClusterClick(cluster.ClusterId)}
              >
                <h4>{cluster.CommonElement}</h4> {/* CommonElement as title */}

                {/* Collapsible summary section */}
                <div className="summary">
                  <strong>Summary:</strong>
                  <p>
                    {expanded[cluster.ClusterId] 
                      ? cluster.Summary 
                      : `${cluster.Summary.slice(0, 100)}...`}  {/* Show first 100 characters if collapsed */}
                  </p>
                  <button onClick={() => toggleSummary(cluster.ClusterId)}>
                    {expanded[cluster.ClusterId] ? 'Show Feedbacks' : 'Read More'}
                  </button>
                </div>

                <p><strong>Similar Feedbacks:</strong> {cluster.SimilarFeedbacks}</p>
                <p><strong>Distinct Customers:</strong> {cluster.DistinctCustomers}</p>

                {/* If this cluster is selected, show feedback items */}
                {selectedClusterId === cluster.ClusterId && (
                  <div className="feedback-list">
                    <h5>Feedback Items:</h5>
                    <ul>
                      {/* Show first 3 feedbacks if not expanded, else show all */}
                      {cluster.FeedbackRecords.slice(0, feedbackExpanded[cluster.ClusterId] ? cluster.FeedbackRecords.length : 3).map((feedback, feedbackIndex) => (
                        <li key={feedbackIndex}>
                          <strong>Title:</strong>{feedback.Title} 
                          <br />
                          <strong>Customer:</strong>{feedback.CustomerName}  <br />
                          <strong>User Story:</strong><em>{feedback.UserStory}</em>
                        </li>
                      ))}
                    </ul>

                    {/* Show More/Less button */}
                    <button onClick={() => toggleFeedback(cluster.ClusterId)}>
                      {feedbackExpanded[cluster.ClusterId] ? 'Show Less' : 'Show More'}
                    </button>

                    {/* Feedback pagination controls */}
                    <div className="feedback-pagination">
                      <button
                        className="pagination-button"
                        onClick={() => prevFeedback()}
                        disabled={feedbackIndex === 0}
                      >
                        Previous Feedback
                      </button>
                      <button
                        className="pagination-button"
                        onClick={() => nextFeedback(cluster.FeedbackRecords.length)}
                        disabled={feedbackIndex >= cluster.FeedbackRecords.length - 3}
                      >
                        Next Feedback
                      </button>
                    </div>
                  </div>
                )}
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

      <button onClick={goBack} className="back-button">Back to Main Page</button>

      {/* Embedding ThemeAnalysis and IssueAnalysis Components */}
      <ThemeAnalysis serviceName={serviceName} />
      <IssueAnalysis serviceName={serviceName} />
    </div>
  );
}

export default ServiceClusters;