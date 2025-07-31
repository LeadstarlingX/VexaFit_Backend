export const mockStats = {
    totalUsers: 1250,
    totalWorkouts: 340,
    totalExercises: 150,
    activeUsersToday: 89,
};

export const mockRecentUsers = [
    { id: 1, name: 'Alice Johnson', email: 'alice@example.com', role: 'Admin', joined: '2024-07-28', status: 'Active' },
    { id: 2, name: 'Bob Williams', email: 'bob@example.com', role: 'Trainee', joined: '2024-07-27', status: 'Active' },
];

export const mockAllUsers = [
    ...mockRecentUsers,
    { id: 6, name: 'Frank Castle', email: 'frank@example.com', role: 'Trainee', joined: '2024-07-24', status: 'Active' },
];

export const mockCategories = [
    { Id: 1, Name: 'Chest', Type: 0 },
    { Id: 2, Name: 'Legs', Type: 0 },
    { Id: 3, Name: 'Cardio', Type: 1 },
];

export const mockExercises = [
    {
        Id: 1,
        Name: 'Push-up',
        Description: 'A classic bodyweight exercise.',
        Image: [{ Url: 'https://placehold.co/100x100/0088FE/FFFFFF?text=Push-up' }],
        Categories: [mockCategories[0]]
    },
    {
        Id: 2,
        Name: 'Squat',
        Description: 'A fundamental lower body exercise.',
        Image: [{ Url: 'https://placehold.co/100x100/00C49F/FFFFFF?text=Squat' }],
        Categories: [mockCategories[1]]
    },
];


export const mockWorkouts = [
    {
        Id: 1,
        Name: 'Full Body Strength',
        Description: 'A simple workout to target all major muscle groups.',
        WorkoutExercises: [
            { Exercise: mockExercises[0] }, 
            { Exercise: mockExercises[1] }  
        ]
    },
    {
        Id: 2,
        Name: 'Quick Cardio Blast',
        Description: 'A 5-minute cardio workout.',
        WorkoutExercises: []
    }
];


export const userActivityData = [
    { name: 'Jan', users: 400 }, { name: 'Feb', users: 300 }, { name: 'Mar', users: 500 },
];

export const roleDistributionData = [
    { name: 'Trainees', value: 1150 },
    { name: 'Admins', value: 100 },
];

export const PIE_CHART_COLORS = ['#0088FE', '#FF8042'];
