import React, { useState, useEffect } from 'react';
import { LineChart, Line, PieChart, Pie, CartesianGrid, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer, Cell } from 'recharts';
import { Users, Dumbbell, Activity, UserPlus } from 'lucide-react';
import StatCard from '../../components/StatCard/StatCard.jsx';
import StatusBadge from '../../components/StatusBadge/StatusBadge.jsx';
import { dataService } from '../../services/dataService.js';
import { roleDistributionData, userActivityData, PIE_CHART_COLORS } from '../../data/mockData.jsx';
import './DashboardPage.css';

const DashboardPage = () => {
    const [stats, setStats] = useState(null);
    const [recentUsers, setRecentUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const statsData = await dataService.getDashboardStats();
                const recentUsersData = await dataService.getRecentUsers();

                setStats(statsData);
                setRecentUsers(recentUsersData);
                setError(null);
            } catch (err) {
                setError("Failed to load dashboard data.");
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    if (loading) {
        return <div className="page-content"><h2>Loading Dashboard...</h2></div>;
    }
    if (error) {
        return <div className="page-content"><h2 style={{ color: 'red' }}>{error}</h2></div>;
    }

    return (
        <div>
            <h2 className="page-title">Dashboard Overview</h2>
            <div className="dashboard-grid">
                <StatCard title="Total Users" value={stats?.TotalUsers ?? stats?.totalUsers ?? 'N/A'} icon={Users} color="var(--primary-blue)" />
                <StatCard title="Total Workouts" value={stats?.TotalWorkouts ?? stats?.totalWorkouts ?? 'N/A'} icon={Dumbbell} color="var(--primary-green)" />
                <StatCard title="Total Exercises" value={stats?.TotalExercises ?? stats?.totalExercises ?? 'N/A'} icon={Activity} color="var(--primary-yellow)" />
                <StatCard title="Active Users Today" value={stats?.ActiveUsersToday ?? stats?.activeUsersToday ?? 'N/A'} icon={UserPlus} color="var(--primary-orange)" />
            </div>

            <div className="charts-grid">
                <div className="card chart-span-2">
                    <h3 className="card-title">User Growth</h3>
                    <ResponsiveContainer width="100%" height={300}>
                        <LineChart data={userActivityData}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="name" />
                            <YAxis />
                            <Tooltip />
                            <Legend />
                            <Line type="monotone" dataKey="users" stroke="#8884d8" strokeWidth={2} activeDot={{ r: 8 }} />
                        </LineChart>
                    </ResponsiveContainer>
                </div>
                <div className="card">
                    <h3 className="card-title">Role Distribution</h3>
                    <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                            <Pie data={roleDistributionData} cx="50%" cy="50%" labelLine={false} outerRadius={100} fill="#8884d8" dataKey="value" label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}>
                                {roleDistributionData.map((entry, index) => (
                                    <Cell key={`cell-${index}`} fill={PIE_CHART_COLORS[index % PIE_CHART_COLORS.length]} />
                                ))}
                            </Pie>
                            <Tooltip />
                        </PieChart>
                    </ResponsiveContainer>
                </div>
            </div>

            <div className="card" style={{ marginTop: '1.5rem' }}>
                <h3 className="card-title">Recent Users</h3>
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Role</th>
                                <th>Joined Date</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {recentUsers.map(user => (
                                <tr key={user.Id ?? user.id}>
                                    <td>
                                        <div>{user.UserName ?? user.name}</div>
                                        <div style={{ fontSize: '0.875rem', color: 'var(--gray-500)' }}>{user.Email ?? user.email}</div>
                                    </td>
                                    <td>{user.Roles ? user.Roles.join(', ') : user.role}</td>
                                    <td>{new Date(user.JoinedDate ?? user.joined).toLocaleDateString()}</td>
                                    <td><StatusBadge status={user.IsActive ? 'Active' : (user.status || 'Inactive')} /></td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default DashboardPage;
