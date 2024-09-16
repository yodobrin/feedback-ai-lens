import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import ServiceHighlights from './components/ServiceHighlights';
import ServiceClusters from './components/ServiceClusters';
import ThemeAnalysis from './components/ThemeAnalysis';
import IssueAnalysis from './components/IssueAnalysis';
import './App.css'; // Global styles

function App() {
  const [selectedService, setSelectedService] = useState('');

  const handleServiceSelect = (serviceName) => {
    setSelectedService(serviceName);
  };

  return (
    <Router>
      <Routes>
        {/* Main highlights page */}
        <Route path="/" element={<ServiceHighlights onServiceSelect={handleServiceSelect} />} />
        
        {/* Cluster page with parameterized serviceName */}
        <Route path="/service-clusters/:serviceName" element={<ServiceClusters />} />
        
        {/* Theme Analysis and Issue Analysis pages */}
        <Route path="/theme-analysis" element={<ThemeAnalysis />} />
        <Route path="/issue-analysis" element={<IssueAnalysis />} />
      </Routes>
    </Router>
  );
}

export default App;