import api from './api';

export const login = async (email, password) => {
    try {
        // Sends the POST with a json on the body
        const response = await api.post('/auth/login', {
            email,
            password,
        });

        // Deconstruct the tokens
        const { accessToken, refreshToken } = response.data;

        // Save both tokens on the browser local storage
        if (accessToken && refreshToken) {
            localStorage.setItem('accessToken', accessToken);
            localStorage.setItem('refreshToken', refreshToken);
        }

        return response.data;
    } catch (error) {
        if (error.response) {
            throw new Error(error.response.data.message || 'Invalid credentials.')
        } else if (error.request) {
            throw new Error('Could not connect to the server.')
        } else {
            throw new Error('Error while trying to login.')
        }
    }
};

export const logout = async () => {
    try {
        await api.post('/auth/logout');
    } catch (error) {
        console.error('Erro ao fazer logout no servidor:', error);
    } finally {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }
};