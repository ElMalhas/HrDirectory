import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5151/api',
  headers: {
    'Content-Type': 'application/json',
  }
});

// Request Interceptor
api.interceptors.request.use(
  (config) => {
    // Search token on the local storage
    const token = localStorage.getItem('accessToken');

    // Injects the Authorization header (Corrigido para backticks)
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;