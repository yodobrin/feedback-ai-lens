import React, { useState } from 'react';

function ThemeAnalysis({ serviceName }) {
  const [issue, setIssue] = useState('');  // Issue entered by the user
  const [issueResult, setIssueResult] = useState(null);  // API result
  const [error, setError] = useState(null);

  // Handle Analyze button click
  const handleIssueAnalyze = () => {
    if (!issue) {
      setError('Please enter an issue to analyze');
      return;
    }

    // Call the API to get the issue summary
    fetch(`http://localhost:5229/api/Services/GetSummaryByIssue/${serviceName}?userQuery=${encodeURIComponent(issue)}`)
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch the issue summary');
        }
        return response.json();
      })
      .then(data => {
        setIssueResult(data);  // Set result from API
        setError(null);  // Clear previous errors
      })
      .catch(error => {
        console.error('Error fetching issue summary:', error);
        setError('Failed to load issue summary');
      });
  };

  return (
    <div className="theme-analyze">
      <h3>Summarize Feedback by Issue</h3>
      <textarea
        rows="4"
        placeholder="Enter feedback issue"
        value={issue}
        onChange={(e) => setIssue(e.target.value)}
      />
      <button onClick={handleIssueAnalyze}>Analyze</button>

      {error && <div className="error-message">{error}</div>}

      {/* Display the API result */}
      {issueResult && (
        <div className="issue-result">
          <h4>Summary for "{issueResult.issue}"</h4>
          <p>Similar Issues: {issueResult.similar_issues}</p>
          <p>Distinct Customers: {issueResult.distinct_customers}</p>
          <h5>Feedback Links:</h5>
          <ul>
            {issueResult.feedback_links.map((link, index) => (
              <li key={index}><a href={link} target="_blank" rel="noopener noreferrer">{link}</a></li>
            ))}
          </ul>
          <p>{issueResult.summary}</p>
        </div>
      )}
    </div>
  );
}

export default ThemeAnalysis;