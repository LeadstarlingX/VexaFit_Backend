import React, { useState } from 'react';
import Sidebar from './components/Sidebar/Sidebar.jsx';
import Header from './components/Header/Header.jsx';
import DashboardPage from './pages/DashboardPage/DashboardPage.jsx';
import UsersPage from './pages/UsersPage/UsersPage.jsx';
import WorkoutsPage from './pages/WorkoutsPage/WorkoutsPage.jsx';
import SettingsPage from './pages/SettingsPage/SettingsPage.jsx';

// Import the new CSS files
import './App.css';

export default function App() {
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [currentPage, setCurrentPage] = useState('dashboard');

    const renderPage = () => {
        switch (currentPage) {
            case 'dashboard':
                return <DashboardPage />;
            case 'users':
                return <UsersPage />;
            case 'workouts':
                return <WorkoutsPage />;
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
