import React from 'react';
import { Settings } from 'lucide-react';
import './SettingsPage.css';
import '../UsersPage/UsersPage.css'; // Re-using .page-title

const SettingsPage = () => (
    <div>
        <h2 className="page-title">Settings</h2>
        <div className="card placeholder-page">
            <Settings size={48} className="placeholder-icon" />
            <h3 className="placeholder-title">Settings Page</h3>
            <p className="placeholder-text">Configuration options for the admin dashboard will be available here.</p>
        </div>
    </div>
);

export default SettingsPage;
