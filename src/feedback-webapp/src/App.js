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
        <Route path="/" element={<ServiceHighlights onServiceSelect={handleServiceSelect} />} />
        <Route path="/service-clusters/:serviceName" element={<ServiceClusters />} />
        <Route path="/clusters" element={<ServiceClusters serviceName={selectedService} />} />
        <Route path="/theme-analysis" element={<ThemeAnalysis serviceName={selectedService} />} />
        <Route path="/issue-analysis" element={<IssueAnalysis serviceName={selectedService} />} />
      </Routes>
    </Router>
  );
}

export default App;