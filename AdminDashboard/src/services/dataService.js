import apiClient from '../api/apiClient';
import {
    mockStats,
    mockAllUsers,
    mockCategories,
    mockExercises, 
    mockWorkouts  
} from '../data/mockData.jsx';

const useMockData = import.meta.env.VITE_USE_MOCK_DATA === 'true';


const getLiveDashboardStats = async () => {
    const response = await apiClient.get('/User/GetDashboardStats/stats');
    return response.data.Data;
};

const getLiveAllUsers = async () => {
    const response = await apiClient.get('/User/GetAllUsers/all');
    return response.data.Data;
};

const toggleLiveUserStatus = async (userId) => {
    const response = await apiClient.put('/User/ToggleUserStatus/toggle-status', { userId });
    return response.data;
};



const getLiveCategories = async () => {
    const response = await apiClient.get('/Category/GetAll');
    return response.data.Data;
};

const createLiveCategory = async (categoryData) => {
    const params = new URLSearchParams({ Name: categoryData.Name, Type: categoryData.Type });
    const response = await apiClient.post(`/Category/Insert?${params.toString()}`);
    return response.data;
};

const updateLiveCategory = async (categoryData) => {
    const params = new URLSearchParams({ Id: categoryData.Id, Name: categoryData.Name, Type: categoryData.Type });
    const response = await apiClient.put(`/Category/Update?${params.toString()}`);
    return response.data;
};

const deleteLiveCategory = async (id) => {
    const response = await apiClient.delete(`/Category/Delete?Id=${id}`);
    return response.data;
};



const getLiveExercises = async () => {
    const response = await apiClient.get('/Exercise/GetAll');
    return response.data.Data;
};

const createLiveExercise = async (exerciseData) => {
    const formData = new FormData();
    formData.append('Name', exerciseData.Name);
    formData.append('Description', exerciseData.Description);
    exerciseData.CategoryIds.forEach(id => formData.append('CategoryId', id));
    if (exerciseData.ImageFile) {
        formData.append('ImageFiles', exerciseData.ImageFile);
    }
    const response = await apiClient.post('/Exercise/Insert', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
};

const updateLiveExercise = async (exerciseData) => {
    const formData = new FormData();
    formData.append('Id', exerciseData.Id);
    formData.append('Name', exerciseData.Name);
    formData.append('Description', exerciseData.Description);
    const response = await apiClient.put('/Exercise/Update', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    return response.data;
};

const deleteLiveExercise = async (id) => {
    const response = await apiClient.delete(`/Exercise/Delete?Id=${id}`);
    return response.data;
};



const getLiveWorkoutById = async (id) => { 
    const response = await apiClient.get(`/Workout/GetById?Id=${id}`);
    return response.data.Data;
};

const getLiveWorkouts = async () => {
    const response = await apiClient.get('/Workout/GetAll');
    return response.data.Data;
};

const createLiveWorkout = async (workoutData) => {
    const params = new URLSearchParams({ Name: workoutData.Name, Description: workoutData.Description });
    const response = await apiClient.post(`/Workout/Insert?${params.toString()}`);
    return response.data;
};

const updateLiveWorkout = async (workoutData) => {
    const params = new URLSearchParams({ Id: workoutData.Id, Name: workoutData.Name, Description: workoutData.Description });
    const response = await apiClient.put(`/Workout/Update?${params.toString()}`);
    return response.data;
};

const deleteLiveWorkout = async (id) => {
    const response = await apiClient.delete(`/Workout/Delete?Id=${id}`);
    return response.data;
};



const addExerciseToLiveWorkout = async (payload) => {
    const params = new URLSearchParams({
        workoutId: payload.workoutId,
        exerciseId: payload.exerciseId,
        Sets: payload.sets,
        Reps: payload.reps,
    });
    const response = await apiClient.post(`/Workout/AddToWorkout?${params.toString()}`);
    return response.data;
};

const removeExerciseFromLiveWorkout = async (workoutExerciseId) => {
    const response = await apiClient.put(`/Workout/DeleteFromWorkout?Id=${workoutExerciseId}`);
    return response.data;
};



const getMockDashboardStats = () => Promise.resolve(mockStats);

const getMockAllUsers = () => Promise.resolve(mockAllUsers);

const toggleMockUserStatus = (userId) => {
    console.log(`Mock: Toggling status for user ID: ${userId}`);
  
    const user = mockAllUsers.find(u => u.id === userId || u.Id === userId);
    if (user) {
        user.IsActive = !user.IsActive;
        user.status = user.IsActive ? 'Active' : 'Inactive';
    }
    return Promise.resolve({ Result: true, Message: "User status updated successfully." });
};


const getMockCategories = () => Promise.resolve(mockCategories);

const createMockCategory = (data) => {
    console.log("Mock: Creating category", data);
    return Promise.resolve({ Result: true });
};

const updateMockCategory = (data) => {
    console.log("Mock: Updating category", data);
    return Promise.resolve({ Result: true });
};

const deleteMockCategory = (id) => {
    console.log("Mock: Deleting category ID:", id);
    return Promise.resolve({ Result: true });
};



const getMockExercises = () => Promise.resolve(mockExercises);

const createMockExercise = (data) => { console.log("Mock: Creating exercise", data); return Promise.resolve({ Result: true }); };

const updateMockExercise = (data) => { console.log("Mock: Updating exercise", data); return Promise.resolve({ Result: true }); };

const deleteMockExercise = (id) => { console.log("Mock: Deleting exercise ID:", id); return Promise.resolve({ Result: true }); };



const getMockWorkoutById = (id) => { 
    return Promise.resolve(mockWorkouts.find(w => w.Id === id));
};

const getMockWorkouts = () => Promise.resolve(mockWorkouts);

const createMockWorkout = (data) => { console.log("Mock: Creating workout", data); return Promise.resolve({ Result: true }); };

const updateMockWorkout = (data) => { console.log("Mock: Updating workout", data); return Promise.resolve({ Result: true }); };

const deleteMockWorkout = (id) => { console.log("Mock: Deleting workout ID:", id); return Promise.resolve({ Result: true }); };



const addExerciseToMockWorkout = (payload) => {
    console.log("Mock: Adding exercise to workout", payload);
    return Promise.resolve({ Result: true });
};

const removeExerciseFromMockWorkout = (workoutExerciseId) => {
    console.log("Mock: Removing workout exercise with ID:", workoutExerciseId);
    return Promise.resolve({ Result: true });
};



export const dataService = {
    getDashboardStats: useMockData ? getMockDashboardStats : getLiveDashboardStats,
    getAllUsers: useMockData ? getMockAllUsers : getLiveAllUsers,
    getRecentUsers: async () => {
        const allUsers = await (useMockData ? getMockAllUsers() : getLiveAllUsers());
        return allUsers.sort((a, b) => new Date(b.JoinedDate || b.joined) - new Date(a.JoinedDate || a.joined)).slice(0, 5);
    },

    toggleUserStatus: useMockData ? toggleMockUserStatus : toggleLiveUserStatus,

    getCategories: useMockData ? getMockCategories : getLiveCategories,
    createCategory: useMockData ? createMockCategory : createLiveCategory,
    updateCategory: useMockData ? updateMockCategory : updateLiveCategory,
    deleteCategory: useMockData ? deleteMockCategory : deleteLiveCategory,

    getExercises: useMockData ? getMockExercises : getLiveExercises,
    createExercise: useMockData ? createMockExercise : createLiveExercise,
    updateExercise: useMockData ? updateMockExercise : updateLiveExercise,
    deleteExercise: useMockData ? deleteMockExercise : deleteLiveExercise,

    getWorkoutById: useMockData ? getMockWorkoutById : getLiveWorkoutById,
    getWorkouts: useMockData ? getMockWorkouts : getLiveWorkouts,
    createWorkout: useMockData ? createMockWorkout : createLiveWorkout,
    updateWorkout: useMockData ? updateMockWorkout : updateLiveWorkout,
    deleteWorkout: useMockData ? deleteMockWorkout : deleteLiveWorkout,


    addExerciseToWorkout: useMockData ? addExerciseToMockWorkout : addExerciseToLiveWorkout,
    removeExerciseFromWorkout: useMockData ? removeExerciseFromMockWorkout : removeExerciseFromLiveWorkout,
};
