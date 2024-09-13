import React, { useState } from 'react';
import './IssueAnalysis.css'; // Ensure you have this for styling

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
   
      {error && <div className="error-message">{error}</div>}

      {/* Display the issue data if available */}
      {issueData && (
        <>
          <div className="user-story-card">
            <h4>Common Theme User Story</h4>
            {issueData.user_story}
          </div>

          {/* Heading for Impacted Customers */}
          <h3 className="impacted-customers-heading">Impacted Customers</h3>

          {/* Ensure customers array exists before using .map() */}
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