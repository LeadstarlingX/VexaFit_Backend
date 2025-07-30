import axios from 'axios';

// IMPORTANT: Replace this with the actual URL of your running backend
const API_BASE_URL = 'http://localhost:5232/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
});

// This "interceptor" will automatically add the authorization token
// to every request after an admin has logged in.
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('admin_token'); // Or wherever you store the token
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
