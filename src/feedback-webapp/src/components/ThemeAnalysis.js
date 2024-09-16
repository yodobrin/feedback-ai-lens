import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import './ThemeAnalysis.css';

function ThemeAnalysis() {
  const [issue, setIssue] = useState('');
  const [issueResult, setIssueResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [feedbackLinksExpanded, setFeedbackLinksExpanded] = useState(false); // State to control feedback links expansion

  const location = useLocation();
  const navigate = useNavigate();
  const { serviceName } = location.state || {};  // Extract serviceName from navigation state

  useEffect(() => {
    if (!serviceName) {
      navigate('/'); // If no serviceName, go back to main page
    }
  }, [serviceName, navigate]);

  const handleIssueAnalyze = () => {
    if (!issue) {
      setError('Please enter an issue to analyze');
      return;
    }

    setLoading(true);
    fetch(`http://localhost:5229/api/Services/GetSummaryByIssue/${serviceName}?userQuery=${encodeURIComponent(issue)}`)
      .then(response => {
        if (!response.ok) throw new Error('Failed to fetch the issue summary');
        return response.json();
      })
      .then(data => {
        setIssueResult(data);
        setError(null);
      })
      .catch(() => {
        setError('Failed to load issue summary');
      })
      .finally(() => setLoading(false));
  };

  const toggleFeedbackLinks = () => {
    setFeedbackLinksExpanded(!feedbackLinksExpanded); // Toggle the expansion
  };

  const goBack = () => {
    navigate(-1); // Go back to the previous page (Service Clusters)
  };

  const goToMainPage = () => {
    navigate('/'); // Navigate back to the main page (Service Highlights)
  };

  return (
    <div className="theme-analyze">
      <h3>Summarize Feedback by Issue for {serviceName}</h3>
      <div className="search-container">
        <textarea
          rows="4"
          placeholder="Enter feedback issue"
          value={issue}
          onChange={(e) => setIssue(e.target.value)}
          className="issue-textarea"
        />
        <button onClick={handleIssueAnalyze} className="search-button">Analyze</button>
      </div>

      {loading && <div className="loading-indicator">Processing...</div>}
      {error && <div className="error-message">{error}</div>}
      {issueResult && (
        <div className="issue-result">
          <h4>{issueResult.title || 'No Title Available'}</h4>

          {issueResult.summary?.main_points && issueResult.summary.main_points.length > 0 ? (
            issueResult.summary.main_points.map((point, index) => (
              <div key={index} className="main-point">
                <h5>{point.title || 'No Title'}</h5>
                <ul>
                  {point.description?.map((desc, i) => (
                    <li key={i}>{desc || 'No Description'}</li>
                  ))}
                </ul>
              </div>
            ))
          ) : (
            <p>No main points available.</p>
          )}

          <p>Similar Issues: {issueResult.similar_issues}</p>
          <p>Distinct Customers: {issueResult.distinct_customers}</p>

          <h5>
            Feedback Links ({issueResult.feedback_links?.length || 0})
            <button onClick={toggleFeedbackLinks} className="toggle-button">
              {feedbackLinksExpanded ? 'Hide' : 'Show'}
            </button>
          </h5>

          {feedbackLinksExpanded && (
            <ul>
              {issueResult.feedback_links?.length > 0 ? (
                issueResult.feedback_links.map((link, index) => (
                  <li key={index}><a href={link} target="_blank" rel="noopener noreferrer">{link}</a></li>
                ))
              ) : (
                <p>No feedback links available.</p>
              )}
            </ul>
          )}
        </div>
      )}

      <div className="navigation-buttons">
        <button onClick={goBack} className="back-button">Back</button>
        <button onClick={goToMainPage} className="main-page-button">Back to Main Page</button>
      </div>
      <img src="/hacklogo.png" alt="Logo" className="bottom-right-logo" />
    </div>
  );
}

export default ThemeAnalysis;
