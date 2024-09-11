import React, { useState } from 'react';

function IssueAnalysis({ serviceName }) {
  const [issue, setIssue] = useState('');  // Issue entered by the user
  const [issueData, setIssueData] = useState(null);  // Holds the full IssueData object
  const [error, setError] = useState(null);

  // Handle the customer search based on the issue
  const handleCustomerSearch = () => {
    if (!issue) {
      setError('Please enter an issue to search');
      return;
    }

    // Call the API to get customers impacted by the issue
    fetch(`http://localhost:5229/api/Services/GetCustomersByIssue/${serviceName}?userQuery=${encodeURIComponent(issue)}`)
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch customers');
        }
        return response.json();
      })
      .then(data => {
        setIssueData(data);  // Set the entire IssueData object from the API response
        setError(null);  // Clear previous errors
      })
      .catch(error => {
        console.error('Error fetching customers:', error);
        setError('Failed to load customers');
      });
  };

  return (
    <div className="issue-analyze">
      <h3>Show me the customers impacted by <em>Issue</em></h3>
      <textarea
        rows="2"
        placeholder="Enter issue"
        value={issue}
        onChange={(e) => setIssue(e.target.value)}
      />
      <button onClick={handleCustomerSearch}>Search</button>

      {error && <div className="error-message">{error}</div>}

      {/* Display the issue data if available */}
      {issueData && (
        <>
          <div className="user-story">
            <h4>Original User Prompt</h4>
            <p>{issueData.original_user_prompt}</p>
          </div>
          <div className="user-story">
            <h4>User Story</h4>
            <p>{issueData.user_story}</p>
          </div>

          {/* Ensure customers array exists before using .map() */}
          {issueData.customers && issueData.customers.length > 0 ? (
            <div className="customer-list">
              <h4>Customers impacted by "{issue}"</h4>
              <ul>
                {issueData.customers.map((customer, index) => (
                  <li key={index}>
                    <strong>{customer.name}</strong> - {customer.industry} (TPID: {customer.tpid})
                  </li>
                ))}
              </ul>
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