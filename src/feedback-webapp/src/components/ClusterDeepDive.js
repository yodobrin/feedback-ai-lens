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
  const goToHome = () => {
    navigate('/'); // Navigate to the home page
  };

  // Helper function to split and display user stories
  const renderUserStories = (userStory) => {
    // Split the userStory string by newlines
    const stories = userStory.split('\n\n').filter(story => story.trim() !== ''); // Split on double newlines and remove empty items
    return (
      <ul>
        {stories.map((story, index) => (
          <li key={index}>
            <em>{story}</em>
          </li>
        ))}
      </ul>
    );
  };
  return (
    <div className="cluster-deep-dive">
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
            <strong>User Story(s):</strong>
            {renderUserStories(feedback.UserStory)}  {/* Call helper function to render user stories */}
          </li>
        ))}
      </ul>

      <button onClick={goBack} className="back-button">Back to Clusters</button>
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

export default ClusterDeepDive;
