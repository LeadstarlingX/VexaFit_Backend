import React, { useState, useEffect } from 'react';
import { Search, UserPlus } from 'lucide-react';
import StatusBadge from '../../components/StatusBadge/StatusBadge.jsx';
// ✨ 1. We import the dataService to handle all data fetching.
import { dataService } from '../../services/dataService.js';
import './UsersPage.css';

const UsersPage = () => {
    const [allUsers, setAllUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                // ✨ 2. We call the dataService to get all users.
                // This will use the apiClient for real data or return mock data,
                // depending on the .env configuration.
                const usersData = await dataService.getAllUsers();
                setAllUsers(usersData);
                setFilteredUsers(usersData);
                setError(null);
            } catch (err) {
                setError("Failed to load user data.");
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    useEffect(() => {
        const lowercasedFilter = searchTerm.toLowerCase();
        const filtered = allUsers.filter(user =>
            (user.UserName || user.name).toLowerCase().includes(lowercasedFilter) ||
            (user.Email || user.email).toLowerCase().includes(lowercasedFilter)
        );
        setFilteredUsers(filtered);
    }, [searchTerm, allUsers]);

    if (loading) return <div className="page-content"><h2>Loading Users...</h2></div>;
    if (error) return <div className="page-content"><h2 style={{ color: 'red' }}>{error}</h2></div>;

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
                            {/* ✨ 3. The UI is populated with the fetched user data. */}
                            {filteredUsers.map(user => (
                                <tr key={user.Id ?? user.id}>
                                    <td>
                                        <div>{user.UserName ?? user.name}</div>
                                        <div style={{ fontSize: '0.875rem', color: 'var(--gray-500)' }}>{user.Email ?? user.email}</div>
                                    </td>
                                    <td>{user.Roles ? user.Roles.join(', ') : user.role}</td>
                                    <td>{new Date(user.JoinedDate ?? user.joined).toLocaleDateString()}</td>
                                    <td><StatusBadge status={user.IsActive ? 'Active' : (user.status || 'Inactive')} /></td>
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
