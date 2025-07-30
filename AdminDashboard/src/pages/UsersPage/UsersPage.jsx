import React, { useState, useEffect, useCallback } from 'react';
import { Search, UserPlus, UserCheck, UserX } from 'lucide-react';
import StatusBadge from '../../components/StatusBadge/StatusBadge.jsx';
import { dataService } from '../../services/dataService.js';
import './UsersPage.css';

const UsersPage = () => {
    const [allUsers, setAllUsers] = useState([]);
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    const fetchUsers = useCallback(async () => {
        try {
            const usersData = await dataService.getAllUsers();
            setAllUsers(usersData);
            setError(null);
        } catch (err) {
            setError("Failed to load user data.");
            console.error(err);
        }
    }, []); // Empty dependency array means the function is created only once.

    useEffect(() => {
        const initialLoad = async () => {
            setLoading(true);
            await fetchUsers();
            setLoading(false);
        };
        initialLoad();
    }, [fetchUsers]); // Now depends on the stable fetchUsers function

    useEffect(() => {
        const lowercasedFilter = searchTerm.toLowerCase();
        const filtered = allUsers.filter(user =>
            (user.UserName || user.name).toLowerCase().includes(lowercasedFilter) ||
            (user.Email || user.email).toLowerCase().includes(lowercasedFilter)
        );
        setFilteredUsers(filtered);
    }, [searchTerm, allUsers]);

    const handleToggleStatus = async (userId) => {
        try {
            await dataService.toggleUserStatus(userId);
            // ✨ 3. Now this call will work correctly
            await fetchUsers();
        } catch (err) {
            alert('Failed to update user status.');
            console.error(err);
        }
    };

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
                            {filteredUsers.map(user => {
                                const isActive = user.IsActive ?? (user.status === 'Active');
                                return (
                                    <tr key={user.Id ?? user.id}>
                                        <td>
                                            <div>{user.UserName ?? user.name}</div>
                                            <div style={{ fontSize: '0.875rem', color: 'var(--gray-500)' }}>{user.Email ?? user.email}</div>
                                        </td>
                                        <td>{user.Roles ? user.Roles.join(', ') : user.role}</td>
                                        <td>{new Date(user.JoinedDate ?? user.joined).toLocaleDateString()}</td>
                                        <td><StatusBadge status={isActive ? 'Active' : 'Inactive'} /></td>
                                        <td>
                                            {/* ✨ 3. Conditional button to activate/deactivate */}
                                            {isActive ? (
                                                <button
                                                    className="action-button danger"
                                                    title="Deactivate User"
                                                    onClick={() => handleToggleStatus(user.Id)}
                                                >
                                                    <UserX size={18} />
                                                </button>
                                            ) : (
                                                <button
                                                    className="action-button success"
                                                    title="Reactivate User"
                                                    onClick={() => handleToggleStatus(user.Id)}
                                                >
                                                    <UserCheck size={18} />
                                                </button>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default UsersPage;
