import apiClient from '../api/apiClient';
import {
    mockStats,
    mockAllUsers,
    mockRecentUsers
} from '../data/mockData.jsx';

// Check the environment variable
const useMockData = import.meta.env.VITE_USE_MOCK_DATA === 'true';

// --- Define API-based functions ---
const getLiveDashboardStats = async () => {
    const response = await apiClient.get('/User/GetDashboardStats/stats');
    return response.data.Data; // Adjust based on your ApiResponse structure
};

const getLiveAllUsers = async () => {
    const response = await apiClient.get('/User/GetAllUsers/all');
    return response.data.Data; // Adjust based on your ApiResponse structure
};

// --- Define Mock-based functions ---
// We wrap mock data in a Promise to simulate a real API call
const getMockDashboardStats = () => {
    return Promise.resolve(mockStats);
};

const getMockAllUsers = () => {
    return Promise.resolve(mockAllUsers);
};

// --- Conditionally export the correct functions ---
export const dataService = {
    getDashboardStats: useMockData ? getMockDashboardStats : getLiveDashboardStats,
    getAllUsers: useMockData ? getMockAllUsers : getLiveAllUsers,
    // For recent users, we can just filter the full list in both cases
    getRecentUsers: async () => {
        const allUsers = await (useMockData ? getMockAllUsers() : getLiveAllUsers());
        // Sort by date and take the top 5 (assuming joinedDate is available)
        return allUsers.sort((a, b) => new Date(b.JoinedDate) - new Date(a.JoinedDate)).slice(0, 5);
    }
};
