import React from 'react';
import './StatCard.css';

const StatCard = ({ title, value, icon, color }) => {
    const IconComponent = icon;
    return (
        <div className="card stat-card">
            <div className="stat-card-icon-wrapper" style={{ backgroundColor: color + '20' }}>
                <IconComponent size={28} style={{ color: color }} />
            </div>
            <div>
                <p className="stat-card-title">{title}</p>
                <p className="stat-card-value">{value}</p>
            </div>
        </div>
    );
};

export default StatCard;
