import React from 'react';
import { LineChart, Line, PieChart, Pie, CartesianGrid, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer, Cell } from 'recharts';
import { Users, Dumbbell, Activity, UserPlus } from 'lucide-react';
import StatCard from '../../components/StatCard/StatCard.jsx';
import StatusBadge from '../../components/StatusBadge/StatusBadge.jsx';
import { mockStats, mockRecentUsers, userActivityData, roleDistributionData, PIE_CHART_COLORS } from '../../data/mockData.jsx';
import './DashboardPage.css';

const DashboardPage = () => {
    return (
        <div>
            <h2 className="page-title">Dashboard Overview</h2>
            <div className="dashboard-grid">
                <StatCard title="Total Users" value={mockStats.totalUsers} icon={Users} color="var(--primary-blue)" />
                <StatCard title="Total Workouts" value={mockStats.totalWorkouts} icon={Dumbbell} color="var(--primary-green)" />
                <StatCard title="Total Exercises" value={mockStats.totalExercises} icon={Activity} color="var(--primary-yellow)" />
                <StatCard title="Active Users Today" value={mockStats.activeUsersToday} icon={UserPlus} color="var(--primary-orange)" />
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
                            {mockRecentUsers.slice(0, 5).map(user => (
                                <tr key={user.id}>
                                    <td>
                                        <div>{user.name}</div>
                                        <div style={{ fontSize: '0.875rem', color: 'var(--gray-500)' }}>{user.email}</div>
                                    </td>
                                    <td>{user.role}</td>
                                    <td>{user.joined}</td>
                                    <td><StatusBadge status={user.status} /></td>
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
