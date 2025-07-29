import React, { useState, useEffect } from 'react';
import { Search, UserPlus } from 'lucide-react';
import StatusBadge from '../../components/StatusBadge/StatusBadge.jsx';
import { mockAllUsers } from '../../data/mockData.jsx';
import './UsersPage.css';

const UsersPage = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const [filteredUsers, setFilteredUsers] = useState(mockAllUsers);

    useEffect(() => {
        const lowercasedFilter = searchTerm.toLowerCase();
        const filtered = mockAllUsers.filter(user =>
            user.name.toLowerCase().includes(lowercasedFilter) ||
            user.email.toLowerCase().includes(lowercasedFilter)
        );
        setFilteredUsers(filtered);
    }, [searchTerm]);

    return (
        <div>
            <div className="page-header">
                <h2 className="page-title">User Management</h2>
                <button className="add-button">
                    <UserPlus size={18} className="add-button-icon" /> Add User
                </button>
            </div>
            <div className="card">
                <div className="search-container" style={{ maxWidth: '28rem', marginBottom: '1rem' }}>
                    <input
                        type="text"
                        placeholder="Search by name or email..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                    />
                    <Search className="search-icon" size={18} />
                </div>
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>User</th>
                                <th>Role</th>
                                <th>Joined Date</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.map(user => (
                                <tr key={user.id}>
                                    <td>
                                        <div>{user.name}</div>
                                        <div style={{ fontSize: '0.875rem', color: 'var(--gray-500)' }}>{user.email}</div>
                                    </td>
                                    <td>{user.role}</td>
                                    <td>{user.joined}</td>
                                    <td><StatusBadge status={user.status} /></td>
                                    <td>
                                        <button style={{ color: 'var(--primary-blue)', marginRight: '0.5rem' }}>Edit</button>
                                        <button style={{ color: '#ef4444' }}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default UsersPage;
