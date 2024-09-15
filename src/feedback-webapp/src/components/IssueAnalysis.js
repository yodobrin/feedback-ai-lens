import React, { useState } from 'react';
import './IssueAnalysis.css';

function IssueAnalysis({ serviceName }) {
  const [issue, setIssue] = useState('');
  const [issueData, setIssueData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

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
      .catch(error => {
        setError('Failed to load customers');
      })
      .finally(() => setLoading(false));
  };

  return (
    <div className="issue-analyze">
      <h3>Show me the customers impacted by <em>Issue</em></h3>
      
      <div className="search-container">
        <textarea
          rows="2"
          className="issue-textarea"
          placeholder="Enter issue"
          value={issue}
          onChange={(e) => setIssue(e.target.value)}
        />
        <button onClick={handleCustomerSearch} className="search-button">Search</button>
      </div>
      
      {loading && <div className="loading-indicator">Processing...</div>}
      {error && <div className="error-message">{error}</div>}
      {issueData && !loading &&(
        <>
          <div className="user-story-card">
            <h4>Common Theme User Story</h4>
            {issueData.user_story}
          </div>

          <h3 className="impacted-customers-heading">Impacted Customers</h3>

          {issueData.customers && issueData.customers.length > 0 ? (
            <div className="customer-grid">
              {issueData.customers.map((customer, index) => (
                <div className="customer-card" key={index}>
                  <h5>{customer.name}</h5>
                  <p><strong>Title:</strong> {customer.feedback_title}</p>
                  <p><strong>TPID:</strong> {customer.tpid}</p>
                </div>
              ))}
            </div>
          ) : (
            <p>No customers found for this issue.</p>
          )}
        </>
      )}
    </div>
  );
}

export default IssueAnalysis;