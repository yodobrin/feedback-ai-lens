import React, { useState, useEffect } from 'react';
import './App.css';  // Ensure the CSS is imported

function App() {
  const [serviceHighlights, setServiceHighlights] = useState([]);
  const [serviceClusters, setServiceClusters] = useState([]);
  const [selectedService, setSelectedService] = useState(null);
  const [error, setError] = useState(null);
  const [theme, setTheme] = useState('');  // State to hold the theme input
  const [analysisResult, setAnalysisResult] = useState(null);  // State for analysis result
  const [issue, setIssue] = useState('');  // State to hold the issue input
  const [issueResult, setIssueResult] = useState(null);  // State for issue analysis result

  // Fetch service highlights when the component mounts
  useEffect(() => {
    fetch('http://localhost:5229/api/Services/GetServiceHighlights')
      .then(response => response.json())
      .then(data => setServiceHighlights(data))
      .catch(error => {
        console.error('Error fetching data:', error);
        setError('Failed to load service highlights');
      });
  }, []);

  // Handle clicking a service card to load clusters
  const handleServiceClick = (serviceName) => {
    setSelectedService(serviceName);
    setError(null);  // Clear any previous errors when switching to clusters
    setAnalysisResult(null);  // Reset any previous analysis results
    setIssueResult(null);  // Reset issue analysis results

    fetch(`http://localhost:5229/api/Services/GetServiceClusters/${serviceName}`)
      .then(response => {
        if (!response.ok) {
          throw new Error('Failed to fetch clusters');
        }
        return response.json();
      })
      .then(data => setServiceClusters(data))
      .catch(error => {
        console.error('Error fetching clusters:', error);
        setError(`Failed to load clusters for ${serviceName}`);
      });
  };

  // Handle the back to highlights action
  const handleBackToHighlights = () => {
    setSelectedService(null);
    setError(null);  // Clear the error when going back to the highlights
    setTheme('');  // Clear the theme input when returning
    setIssue('');  // Clear the issue input
    setAnalysisResult(null);  // Clear previous analysis results
    setIssueResult(null);  // Clear previous issue analysis results
  };

  // Handle theme analysis (mock action for now)
  const handleThemeAnalyze = () => {
    if (!theme) {
      setError('Please enter a theme to analyze');
      return;
    }

    // Mock analysis action; here you can add your API call or logic to analyze the theme
    setAnalysisResult(`Analyzing theme "${theme}" for ${selectedService}...`);  // Mock result
  };

  // Handle issue analysis (mock action for now)
  const handleIssueAnalyze = () => {
    if (!issue) {
      setError('Please enter an issue to analyze');
      return;
    }

    // Mock issue action; here you can add your API call or logic to analyze the issue
    setIssueResult(`Fetching customers who submitted feedback about "${issue}" for ${selectedService}...`);  // Mock result
  };

  return (
    <div className="App">
      <h1>Service Highlights</h1>

      {error && <div className="error-message">{error}</div>}

      <div className="cards-container">
        {selectedService === null ? (
          serviceHighlights.map((highlight, index) => (
            <div className="card" key={index} onClick={() => handleServiceClick(highlight.ServiceName)}>
              <h3>{highlight.ServiceName}</h3>
              <p>Total Feedback: {highlight.TotalFeedback}</p>
              <p>Distinct Customers: {highlight.DistinctCustomers}</p>
              <p>Feature Requests: {highlight.FeatureRequests}</p>
              <p>Bugs: {highlight.Bugs}</p>
              <p>Overall Sentiment: {highlight.OverallSentiment}</p>
            </div>
          ))
        ) : (
          <div>
            <button onClick={handleBackToHighlights}>Back to Highlights</button>
            <h2>{selectedService} Clusters</h2>

            {serviceClusters.length === 0 ? (
              <p>No clusters available for {selectedService}</p>
            ) : (
              <div className="cards-container">
                {serviceClusters.map((cluster, index) => (
                  <div className="cluster-card" key={index}>
                    <h3>{cluster.ClusterName}</h3>
                    <p>Common Element: {cluster.CommonElement}</p>
                    <p>Similar Feedbacks: {cluster.SimilarFeedbacks}</p>
                    <p>Distinct Customers: {cluster.DistinctCustomers}</p>
                  </div>
                ))}
              </div>
            )}

            {/* Section for Theme Analysis */}
            <div className="theme-analyze">
              <h3>Summarize Feedback Theme</h3>
              <textarea
                rows="4"
                placeholder="Enter feedback theme"
                value={theme}
                onChange={(e) => setTheme(e.target.value)}
              />
              <button onClick={handleThemeAnalyze}>Analyze</button>

              {analysisResult && <div className="analysis-result">{analysisResult}</div>}
            </div>

            {/* Section for Issue Analysis */}
            <div className="issue-analyze">
              <h3>Show me list of customers who submitted feedback about <em>Issue</em></h3>
              <textarea
                rows="2"
                placeholder="Enter issue"
                value={issue}
                onChange={(e) => setIssue(e.target.value)}
              />
              <button onClick={handleIssueAnalyze}>Search</button>

              {issueResult && <div className="issue-result">{issueResult}</div>}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;