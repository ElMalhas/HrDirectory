import { useState, useEffect } from 'react';
import LoginPage from './pages/LoginPage';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Verifica se o utilizador já tem sessão ao abrir a página
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

  // Se não estiver autenticado, mostra o ecrã de Login
  if (!isAuthenticated) {
    return <LoginPage onLoginSuccess={() => setIsAuthenticated(true)} />;
  }

  // Se estiver autenticado, mostra a aplicação (futura DepartmentsPage)
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