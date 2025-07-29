import React from 'react';
import './StatusBadge.css';

const StatusBadge = ({ status }) => {
    const statusClass = {
        'Active': 'status-active',
        'Inactive': 'status-inactive',
        'Banned': 'status-banned',
    };
    return (
        <span className={`status-badge ${statusClass[status] || ''}`}>
            {status}
        </span>
    );
};

export default StatusBadge;
