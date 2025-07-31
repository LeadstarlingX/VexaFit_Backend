import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Trend, Rate } from 'k6/metrics';

// --- Configuration ---
// IMPORTANT: Change this to your actual backend URL
const BASE_URL = 'http://localhost:5232';

// --- Custom Metrics ---
// We can create custom metrics to track specific business flows
const workoutCreationTime = new Trend('workout_creation_time');
const successfulLogins = new Rate('successful_logins');

// --- Test Options ---
// This configures the load profile of the test
export const options = {
    stages: [
        { duration: '30s', target: 50 }, // Ramp-up to 50 users over 30 seconds
        { duration: '1m', target: 50 },  // Stay at 50 users for 1 minute
        { duration: '10s', target: 0 },  // Ramp-down to 0 users
    ],
    thresholds: {
        // We want 99% of HTTP requests to succeed
        'http_req_failed': ['rate<0.01'],
        // 95% of requests should be below 800ms
        'http_req_duration': ['p(95)<800'],
        // The custom metric for workout creation should also be fast
        'workout_creation_time': ['p(95)<1000'],
    },
};

// --- Setup Function ---
// This runs once before the VUs start. It's perfect for setup tasks like authentication.
export function setup() {
    console.log('Setting up the test...');

    // Generate a unique user for each test run to avoid conflicts
    const uniqueId = new Date().getTime();
    const email = `testuser_${uniqueId}@test.com`;
    const password = 'Password123!';
    const userName = `TestUser${uniqueId}`;

    // 1. Register a new user
    const registerPayload = JSON.stringify({
        userName: userName,
        email: email,
        password: password,
        confirmPassword: password,
        role: 1, // Assuming 1 is a standard user role
    });

    const registerRes = http.post(`${BASE_URL}/api/Auth/Register`, registerPayload, {
        headers: { 'Content-Type': 'application/json' },
    });

    check(registerRes, { 'User registered successfully': (r) => r.status === 200 });

    // 2. Log in to get the auth token
    const loginPayload = JSON.stringify({
        email: email,
        password: password,
    });

    const loginRes = http.post(`${BASE_URL}/api/Auth/Login`, loginPayload, {
        headers: { 'Content-Type': 'application/json' },
    });

    check(loginRes, { 'Logged in successfully': (r) => r.status === 200 });

    // Extract the token from the response. Based on swagger, it's nested.
    const authToken = loginRes.json('data.token.jwtToken');
    if (!authToken) {
        console.error('Failed to retrieve auth token. Aborting test.');
        return;
    }

    console.log('Setup complete. Auth token obtained.');
    successfulLogins.add(1);

    // The token is returned and will be available in the default function's 'data' parameter
    return { token: authToken };
}


// --- VU Function ---
// This is the main function that each virtual user will execute repeatedly.
export default function (data) {
    // Make sure we got the token from the setup function
    if (!data.token) {
        console.log('VU has no token, skipping iteration.');
        return;
    }

    const params = {
        headers: {
            'Authorization': `Bearer ${data.token}`,
            'Content-Type': 'application/json',
        },
    };

    // We use groups to organize related requests in the k6 results
    group('Workout Management Flow', function () {

        // 1. Get all workouts
        group('GET - All Workouts', function () {
            const res = http.get(`${BASE_URL}/api/Workout/GetAll`, params);
            check(res, { 'Get all workouts status is 200': (r) => r.status === 200 });
            sleep(1); // Simulate user think time
        });

        let newWorkoutId = null;

        // 2. Create a new workout
        group('POST - Create Workout', function () {
            const workoutName = `My k6 Workout ${__VU}-${__ITER}`; // Unique name per user/iteration
            const workoutDesc = 'Created by k6 stress test';

            // The endpoint uses query parameters for creation
            const createUrl = `${BASE_URL}/api/Workout/Insert?Name=${encodeURIComponent(workoutName)}&Description=${encodeURIComponent(workoutDesc)}`;

            const res = http.post(createUrl, null, params);

            check(res, { 'Create workout status is 201': (r) => r.status === 201 });
            workoutCreationTime.add(res.timings.duration); // Add timing to our custom metric

            if (res.status === 201 && res.json('data.id')) {
                newWorkoutId = res.json('data.id');
            }
            sleep(1);
        });

        // Only proceed if the workout was created successfully
        if (newWorkoutId) {
            // 3. Get the created workout by ID
            group('GET - Workout by ID', function () {
                const res = http.get(`${BASE_URL}/api/Workout/GetById?Id=${newWorkoutId}`, params);
                check(res, { 'Get workout by ID status is 200': (r) => r.status === 200 });
                sleep(1);
            });

            // 4. Update the workout
            group('PUT - Update Workout', function () {
                const updatedName = `My Updated k6 Workout ${__VU}-${__ITER}`;
                const updateUrl = `${BASE_URL}/api/Workout/Update?Id=${newWorkoutId}&Name=${encodeURIComponent(updatedName)}`;
                const res = http.put(updateUrl, null, params);
                check(res, { 'Update workout status is 200': (r) => r.status === 200 });
                sleep(1);
            });

            // 5. Add an exercise to the workout
            group('POST - Add Exercise to Workout', function () {
                // IMPORTANT: This assumes an exercise with ID=1 exists in your database.
                // Change this ID if necessary.
                const exerciseId = 1;
                const addExerciseUrl = `${BASE_URL}/api/Workout/AddToWorkout?workoutId=${newWorkoutId}&exerciseId=${exerciseId}&Sets=3&Reps=12`;
                const res = http.post(addExerciseUrl, null, params);
                check(res, { 'Add exercise status is 200': (r) => r.status === 200 });
                sleep(1);
            });

            // 6. Delete the workout to clean up
            group('DELETE - Workout', function () {
                const res = http.del(`${BASE_URL}/api/Workout/Delete?Id=${newWorkoutId}`, null, params);
                check(res, { 'Delete workout status is 200': (r) => r.status === 200 });
                sleep(1);
            });
        }
    });
}
