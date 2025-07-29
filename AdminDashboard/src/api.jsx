// In src/api/apiClient.js

import axios from 'axios';

// The base URL of your .NET backend
const API_BASE_URL = 'http://localhost:5232/api'; // Or whatever your backend port is

const apiClient = axios.create({
    baseURL: API_BASE_URL,
});

// ✨ This is a crucial part: an "interceptor"
// It will automatically add the JWT token to every request after the admin logs in.
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('admin_token'); // Get the token from storage
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

export default apiClient;