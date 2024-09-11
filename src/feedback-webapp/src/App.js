import React, { useState, useEffect } from 'react';
import './App.css';
import ServiceHighlights from './components/ServiceHighlights';  // Highlight view component
import ServiceClusters from './components/ServiceClusters';  // Service-specific view component

function App() {
  const [serviceHighlights, setServiceHighlights] = useState([]);
  const [selectedService, setSelectedService] = useState(null);

  useEffect(() => {
    fetch('http://localhost:5229/api/Services/GetServiceHighlights')
      .then(response => response.json())
      .then(data => setServiceHighlights(data))
      .catch(error => console.error('Error fetching data:', error));
  }, []);

  const handleServiceClick = (serviceName) => {
    setSelectedService(serviceName);
  };

  const handleBackToHighlights = () => {
    setSelectedService(null);
  };

  return (
    <div className="App">
      {selectedService === null ? (
        <ServiceHighlights
          serviceHighlights={serviceHighlights}
          onServiceClick={handleServiceClick}
        />
      ) : (
        <ServiceClusters
          selectedService={selectedService}
          onBack={handleBackToHighlights}
        />
      )}
    </div>
  );
}

export default App;