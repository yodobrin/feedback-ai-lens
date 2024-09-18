import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './IssueAnalysis.css';

function IssueAnalysis() {
  const [issue, setIssue] = useState('');
  const [issueData, setIssueData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const location = useLocation();  // Used to retrieve serviceName from the navigation state
  const { serviceName } = location.state || {};  // Extract serviceName from the state

  // If serviceName is not present, redirect back to main page
  useEffect(() => {
    if (!serviceName) {
      navigate('/');
    }
  }, [serviceName, navigate]);

  const handleCustomerSearch = () => {
    if (!issue) {
      setError('Please enter an issue to search');
      return;
    }

    setLoading(true);
    setError(null);

    fetch(`http://localhost:5229/api/Services/GetCustomersByIssue/${serviceName}?userQuery=${encodeURIComponent(issue)}`)
      .then(response => {
        if (!response.ok) throw new Error('Failed to fetch customers');
        return response.json();
      })
      .then(data => {
        setIssueData(data);
        setError(null);
      })
      .catch(() => {
        setError('Issue provided is too specific, No customers found matching the issue, Try expanding your search');
      })
      .finally(() => setLoading(false));
  };

  const goBack = () => {
    navigate(-1); // Go back to the previous page
  };

  const goToHome = () => {
    navigate('/'); // Navigate back to the main page (Service Highlights)
  };

  return (
    <div className="issue-analyze">
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
      <h3>Show me customers who submitted feedback on &lt; specific issue&gt; in {serviceName}</h3>
      <div className="search-container">
        <textarea
          rows="2"
          className="issue-textarea"
          placeholder="Describe the product issue you want to search for"
          value={issue}
          onChange={(e) => setIssue(e.target.value)}
        />
        <button onClick={handleCustomerSearch} className="search-button">Search</button>
      </div>

      {loading && <div className="loading-indicator">Processing...</div>}
      {error && <div className="error-message">{error}</div>}
      {issueData && !loading && (
        <>
          <div className="user-story-card">
            <h4>Common Theme User Story</h4>
            {/* Adjusted to reflect the new structure for displaying the main points of the common user story */}
            {issueData.summary_detail && issueData.summary_detail.main_points.map((point, index) => (
              <div key={index} className="main-point">
                <h5>{point.title}</h5>
                <ul>
                  {point.description.map((desc, i) => (
                    <li key={i}>{desc}</li>
                  ))}
                </ul>
              </div>
            ))}
          </div>

          <h3 className="impacted-customers-heading">
            Impacted Customers ({issueData.customers.length})
          </h3>

          {issueData.customers && issueData.customers.length > 0 ? (
            <div className="customer-list">
              {issueData.customers.map((customer, index) => (
                <div key={index} className="customer-summary-section">
                  <p><strong>Customer:</strong>{customer.name}</p>
                  <p><strong>Main theme:</strong> {customer.feedback_title}</p>

                  {/* Displaying the customer's summary details */}
                  {customer.summary_detail && customer.summary_detail.main_points.map((point, idx) => (
                    <div key={idx} className="main-point">
                      <p><strong>Specific Point(s):</strong>{point.title}</p>
                      <ul>
                        {point.description.map((desc, i) => (
                          <li key={i}>{desc}</li>
                        ))}
                      </ul>
                    </div>
                  ))}
                  <p><strong>Customer TPID:</strong> {customer.tpid}</p>

                  {/* Display feedback records if available */}
                  {customer.feedback_records && customer.feedback_records.length > 0 && (
                    <div className="feedback-records">
                      <p>Feedback Record(s)</p>
                      {/* <h6>Feedback Records</h6> */}
                      <ul>
                        {customer.feedback_records.map((feedback, i) => (
                          <li key={i}>{feedback.UserStory} ({feedback.Id})</li>
                        ))}
                      </ul>
                    </div>
                  )}
                  {/* Horizontal line to separate customers */}
                  <hr />
                </div>
              ))}
            </div>
          ) : (
            <p>No customers found for this issue.</p>
          )}
        </>
      )}

      {/* Navigation buttons */}
      <div className="navigation-buttons">
        <button onClick={goBack} className="back-button">Back</button>
        <button onClick={goToHome} className="main-page-button">Back to Main Page</button>
      </div>
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

export default IssueAnalysis;
