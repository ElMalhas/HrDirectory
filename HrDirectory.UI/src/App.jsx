import { useState, useEffect } from 'react';
import LoginPage from './pages/LoginPage';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Checks if the user is already authenticated
  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      setIsAuthenticated(true);
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    setIsAuthenticated(false);
  };

  // Shows the login screen if the user is not authenticated
  if (!isAuthenticated) {
    return <LoginPage onLoginSuccess={() => setIsAuthenticated(true)} />;
  }

  // Shows the landing page if the user is authenticated
  return (
    <div style={{ padding: '20px', fontFamily: 'sans-serif' }}>
      <h1>HR Directory Frontend</h1>
      <p>Bem-vindo! Estás autenticado com sucesso.</p>
      <button onClick={handleLogout} style={{ padding: '10px', cursor: 'pointer' }}>
        Sair (Logout)
      </button>
    </div>
  );
}

export default App;