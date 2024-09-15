import React, { useState } from 'react';
import './ThemeAnalysis.css';

function ThemeAnalysis({ serviceName }) {
  const [issue, setIssue] = useState('');
  const [issueResult, setIssueResult] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

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
      .catch(error => {
        setError('Failed to load issue summary');
      })
      .finally(() => setLoading(false));
  };

  return (
    <div className="theme-analyze">
      <h3>Summarize Feedback by Issue</h3>
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
          <h4>Summary for "{issueResult.issue}"</h4>
          <p>Similar Issues: {issueResult.similar_issues}</p>
          <p>Distinct Customers: {issueResult.distinct_customers}</p>
          <div className="summary-text">
            <h4>Summary</h4>
            <p>{issueResult.summary}</p>
          </div>
          <h5>Feedback Links:</h5>
          <ul>
            {issueResult.feedback_links.map((link, index) => (
              <li key={index}><a href={link} target="_blank" rel="noopener noreferrer">{link}</a></li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

export default ThemeAnalysis;