// In a real application, this data would come from your API.

export const mockStats = {
    totalUsers: 1250,
    totalWorkouts: 340,
    totalExercises: 150,
    activeUsersToday: 89,
};

export const mockRecentUsers = [
    { id: 1, name: 'Alice Johnson', email: 'alice@example.com', role: 'Admin', joined: '2024-07-28', status: 'Active' },
    { id: 2, name: 'Bob Williams', email: 'bob@example.com', role: 'Trainee', joined: '2024-07-27', status: 'Active' },
    { id: 3, name: 'Charlie Brown', email: 'charlie@example.com', role: 'Trainee', joined: '2024-07-27', status: 'Inactive' },
    { id: 4, name: 'Diana Prince', email: 'diana@example.com', role: 'Trainee', joined: '2024-07-26', status: 'Active' },
    { id: 5, name: 'Ethan Hunt', email: 'ethan@example.com', role: 'Trainee', joined: '2024-07-25', status: 'Banned' },
];

export const mockAllUsers = [
    ...mockRecentUsers,
    { id: 6, name: 'Frank Castle', email: 'frank@example.com', role: 'Trainee', joined: '2024-07-24', status: 'Active' },
    { id: 7, name: 'Grace Hopper', email: 'grace@example.com', role: 'Admin', joined: '2024-07-23', status: 'Active' },
    { id: 8, name: 'Henry Jekyll', email: 'henry@example.com', role: 'Trainee', joined: '2024-07-22', status: 'Inactive' },
    { id: 9, name: 'Ivy Pepper', email: 'ivy@example.com', role: 'Trainee', joined: '2024-07-21', status: 'Active' },
    { id: 10, name: 'Jack Sparrow', email: 'jack@example.com', role: 'Trainee', joined: '2024-07-20', status: 'Active' },
];

export const userActivityData = [
    { name: 'Jan', users: 400 }, { name: 'Feb', users: 300 }, { name: 'Mar', users: 500 },
    { name: 'Apr', users: 450 }, { name: 'May', users: 600 }, { name: 'Jun', users: 700 },
    { name: 'Jul', users: 850 },
];

export const roleDistributionData = [
    { name: 'Trainees', value: 1150 },
    { name: 'Admins', value: 100 },
];

export const PIE_CHART_COLORS = ['#0088FE', '#FF8042'];
