import React, { useState, useEffect } from 'react';
import Sidebar from './components/Sidebar/Sidebar.jsx';
import Header from './components/Header/Header.jsx';
import DashboardPage from './pages/DashboardPage/DashboardPage.jsx';
import UsersPage from './pages/UsersPage/UsersPage.jsx';
import WorkoutsPage from './pages/WorkoutsPage/WorkoutsPage.jsx';
import SettingsPage from './pages/SettingsPage/SettingsPage.jsx';
import LoginPage from './pages/LoginPage/LoginPage.jsx';
// ✨ 1. Import the new pages
import ExercisesPage from './pages/ExercisePage/ExercisePage.jsx';
import CategoriesPage from './pages/CategoriesPage/CategoriesPage.jsx';
import apiClient from './api/apiClient.js';

import './App.css';

export default function App() {
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [currentPage, setCurrentPage] = useState('dashboard');

    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [authError, setAuthError] = useState('');
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const token = localStorage.getItem('admin_token');
        if (token) {
            setIsAuthenticated(true);
        }
    }, []);

    const handleLogin = async ({ email, password }) => {
        setLoading(true);
        setAuthError('');
        try {
            const response = await apiClient.post('/Auth/Login', { email, password });

            const userProfile = response.data.Data;
            const roles = userProfile?.Token?.Roles || [];

            if (roles.includes('Admin')) {
                const token = userProfile.Token.JwtToken;
                localStorage.setItem('admin_token', token);
                setIsAuthenticated(true);
            } else {
                setAuthError('Access denied. Only admins can log in.');
            }
        } catch (err) {
            console.error("Login failed:", err);
            setAuthError('Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        localStorage.removeItem('admin_token');
        setIsAuthenticated(false);
        setCurrentPage('dashboard');
    };

    if (!isAuthenticated) {
        return <LoginPage handleLogin={handleLogin} error={authError} loading={loading} />;
    }

    const renderPage = () => {
        switch (currentPage) {
            case 'dashboard':
                return <DashboardPage />;
            case 'users':
                return <UsersPage />;
            case 'workouts':
                return <WorkoutsPage />;
            // ✨ 2. Add cases for the new pages
            case 'exercises':
                return <ExercisesPage />;
            case 'categories':
                return <CategoriesPage />;
            case 'settings':
                return <SettingsPage />;
            default:
                return <DashboardPage />;
        }
    };

    return (
        <div className="app-container">
            <Sidebar
                isOpen={sidebarOpen}
                setIsOpen={setSidebarOpen}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                handleLogout={handleLogout}
            />
            <div className="main-content">
                <Header setIsOpen={setSidebarOpen} />
                <main className="page-content">
                    {renderPage()}
                </main>
            </div>
        </div>
    );
}
