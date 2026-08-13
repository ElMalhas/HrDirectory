import { useState, useEffect } from 'react';
import api from './services/api';

function App() {
  const [statusMessage, setStatusMessage] = useState('Connecting with API...');

  useEffect(() => {
    api.get('/api/status')
      .then((response) => {
        setStatusMessage(response.data);
      })
      .catch((error) => {
        console.error("Error while connecting with API:", error);
        setStatusMessage('Error: Not possible to connect with API.');
      });
  }, []); 

  return (
    <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
      <h1>HR Directory Frontend</h1>
      <p><strong>API Status:</strong> {statusMessage}</p>
    </div>
  );
}

export default App;