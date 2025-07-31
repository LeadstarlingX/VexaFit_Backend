import http from 'k6/http';
import { check, sleep, group } from 'k6';
import { Trend, Rate } from 'k6/metrics';

// --- Configuration ---
const BASE_URL = 'http://localhost:5232';

// --- Custom Metrics ---
const successfulLogins = new Rate('successful_logins');

// Stage Durations
const Round1 = '5s';
const Round2 = '20s';
const Round3 = '20s';
const Round4 = '10s';
const Round5 = '5s';

// Targets for Scenario 1: power_user_workout_flow
const scenario1target1 = 10;
const scenario1target2 = 50;
const scenario1target3 = 50;
const scenario1target4 = 100;
const scenario1target5 = 0;

// Targets for Scenario 2: browsing_user_flow
const scenario2target1 = 10;
const scenario2target2 = 50;
const scenario2target3 = 50;
const scenario2target4 = 100;
const scenario2target5 = 0;

// Targets for Scenario 3: admin_user_flow
const scenario3target1 = 10;
const scenario3target2 = 20;
const scenario3target3 = 20;
const scenario3target4 = 40;
const scenario3target5 = 0;

// --- Test Options ---
export const options = {
    thresholds: {
        'http_req_failed': ['rate<0.01'],
        'http_req_duration': ['p(95)<800'],
    },
    // Scenarios allow us to run different user flows concurrently
    scenarios: {
        // Scenario 1: A user who creates, updates, and deletes workouts
        power_user_workout_flow: {
            executor: 'ramping-vus',
            exec: 'workoutManagementFlow', // Maps to the function below
            // 5-stage load profile for this scenario
            stages: [
                { duration: Round1, target: scenario1target1 },
                { duration: Round2, target: scenario1target2 },
                { duration: Round3, target: scenario1target3 },
                { duration: Round4, target: scenario1target4 },
                { duration: Round5, target: scenario1target5 },  // 5. Ramp-down
            ],
        },

        // Scenario 2: A user who only browses data (read-only)
        browsing_user_flow: {
            executor: 'ramping-vus',
            exec: 'browsingFlow',
            stages: [
                { duration: Round1, target: scenario2target1 },
                { duration: Round2, target: scenario2target2 },
                { duration: Round3, target: scenario2target3 },
                { duration: Round4, target: scenario2target4 },
                { duration: Round5, target: scenario2target5 },
            ],
        },

        // Scenario 3: An admin user performing admin tasks
        admin_user_flow: {
            executor: 'ramping-vus',
            exec: 'adminFlow',
            stages: [
                { duration: Round1, target: scenario3target1 },
                { duration: Round2, target: scenario3target2 },
                { duration: Round3, target: scenario3target3 },
                { duration: Round4, target: scenario3target4 },
                { duration: Round5, target: scenario3target5 },
            ],
        },
    },
};

// --- Helper Functions ---
function createUser(email, password, userName, role) {
    const registerPayload = JSON.stringify({ userName, email, password, confirmPassword: password, role });
    const registerRes = http.post(`${BASE_URL}/api/Auth/Register`, registerPayload, { headers: { 'Content-Type': 'application/json' } });

    const registerBody = registerRes.json();
    if (!registerBody || !registerBody.Result || registerBody.Code !== 201) {
        throw new Error(`Setup failed: Could not register user ${email}. API Message: ${registerBody ? registerBody.Message : "No response body."}`);
    }
    console.log(`Successfully registered ${role === 0 ? 'Admin' : 'User'}: ${email}`);

    const loginPayload = JSON.stringify({ email, password });
    const loginRes = http.post(`${BASE_URL}/api/Auth/Login`, loginPayload, { headers: { 'Content-Type': 'application/json' } });

    const loginBody = loginRes.json();
    if (!loginBody || !loginBody.Result || !loginBody.Data.Token.JwtToken) {
        throw new Error(`Setup failed: Could not log in as ${email}.`);
    }

    successfulLogins.add(1);
    return loginBody.Data.Token.JwtToken;
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

// --- Setup Function ---
export function setup() {
    console.log('Setting up the test by creating user types...');
    const uniqueId = new Date().getTime();

    const userToken = createUser(`testuser_${uniqueId}@test.com`, 'P@ssw0rd123!', `TestUser${uniqueId}`, 1);
    const adminToken = createUser(`admin_${uniqueId}@test.com`, 'P@ssw0rd123!', `AdminUser${uniqueId}`, 0);

    console.log('Setup complete. Tokens obtained for all user types.');
    return { userToken: userToken, adminToken: adminToken };
}

// --- Scenario 1: Power User ---
export function workoutManagementFlow(data) {
    // Return early if the user token is not available
    if (!data.userToken) {
        return;
    }

    // Set headers for all requests in this flow.
    const params = {
        headers: {
            'Authorization': `Bearer ${data.userToken}`,
            'Content-Type': 'application/json',
        },
    };

    // Simpler params for GET/DELETE requests that don't send a body
    const authHeaderOnly = { headers: { 'Authorization': `Bearer ${data.userToken}` } };

    group('Power User: Full Workout & Exercise Lifecycle', function () {
        let newWorkoutId = null;
        let workoutExerciseId = null;

        // --- SECTION 1: Create Workout ---
        const createUrl = `${BASE_URL}/api/Workout/Insert`;
        const payload = JSON.stringify({
            Name: `My k6 Workout ${__VU}-${__ITER}`,
            Description: 'Created by k6 test'
        });
        const createRes = http.post(createUrl, payload, params);


        let createBody;
        if (createRes.body) { try { createBody = createRes.json(); } catch (e) { createBody = null; } }

        const isWorkoutCreated = check(createRes, { '1. Workout created successfully': (r) => r.status === 200 && createBody && createBody.Result });

        if (!isWorkoutCreated) {
            //console.error(`[CHECK FAILED] 1. Workout Creation Failed: VU=${__VU}, Status=${createRes.status}, Message=${createBody.body}`);
        } else {
            // **THIS IS THE FIX:** The API now returns the ID, so we can extract it.
            if (createBody.Data && createBody.Data.id) {
                newWorkoutId = createBody.Data.id;
            } else {
                //console.error(`[CHECK FAILED] 1a. Workout Created, but response body did not contain Data.id. Body=${createRes.body}`);
            }
        }
        sleep(1);

        // --- SECTION 2: Manage Exercises (only if workout was created) ---
        if (newWorkoutId) {
            // Add Exercise to Workout
            const addExerciseUrl = `${BASE_URL}/api/Workout/AddToWorkout?workoutId=${newWorkoutId}&exerciseId=1&Sets=3&Reps=12`;
            const addRes = http.post(addExerciseUrl, null, authHeaderOnly);
            let addBody;
            if (addRes.body) { try { addBody = addRes.json(); } catch (e) { addBody = null; } }

            const isExerciseAdded = check(addRes, { '2. Exercise added to workout': (r) => r.status === 200 && addBody && addBody.Result });
            if (!isExerciseAdded) {
                //console.error(`[CHECK FAILED] 2. Add Exercise Failed: VU=${__VU}, Status=${addRes.status}, Body=${addRes.body}`);
            }
            sleep(1);

            // Fetch workout details to get the workoutExerciseId
            const getWorkoutRes = http.get(`${BASE_URL}/api/Workout/GetById?Id=${newWorkoutId}`, authHeaderOnly);
            const isWorkoutFetched = check(getWorkoutRes, { '3. Fetched workout details': (r) => r.status === 200 });
            if (!isWorkoutFetched) {
                //console.error(`[CHECK FAILED] 3. Get Workout By ID Failed: VU=${__VU}, Status=${getWorkoutRes.status}, Body=${getWorkoutRes.body}`);
            } else {
                const workoutDetails = getWorkoutRes.json();
                if (workoutDetails && workoutDetails.Data && workoutDetails.Data.workoutExercises.length > 0) {
                    workoutExerciseId = workoutDetails.Data.workoutExercises[0].id;
                } else {
                    //console.error(`[CHECK FAILED] 3a. Workout Fetched, but no exercises found in response: VU=${__VU}, Body=${getWorkoutRes.body}`);
                }
            }
            sleep(1);

            if (workoutExerciseId) {
                // Update Exercise in Workout
                const updateExerciseUrl = `${BASE_URL}/api/Workout/UpdateExerciseInWorkout?WorkoutExerciseId=${workoutExerciseId}&Sets=5&Reps=15`;
                const updateRes = http.put(updateExerciseUrl, null, authHeaderOnly);
                const isExerciseUpdated = check(updateRes, { '4. Updated exercise in workout': (r) => r.status === 200 });
                if (!isExerciseUpdated) {
                    //console.error(`[CHECK FAILED] 4. Update Exercise Failed: VU=${__VU}, Status=${updateRes.status}, Body=${updateRes.body}`);
                }
                sleep(1);

                // Delete Exercise from Workout
                const deleteExerciseUrl = `${BASE_URL}/api/Workout/DeleteFromWorkout?Id=${workoutExerciseId}`;
                const deleteExerciseRes = http.put(deleteExerciseUrl, null, authHeaderOnly);
                const isExerciseDeleted = check(deleteExerciseRes, { '5. Deleted exercise from workout': (r) => r.status === 200 });
                if (!isExerciseDeleted) {
                    //console.error(`[CHECK FAILED] 5. Delete Exercise Failed: VU=${__VU}, Status=${deleteExerciseRes.status}, Body=${deleteExerciseRes.body}`);
                }
                sleep(1);
            }

            // --- SECTION 3: Clean up by deleting the created workout ---
            const deleteWorkoutRes = http.del(`${BASE_URL}/api/Workout/Delete?Id=${newWorkoutId}`, authHeaderOnly);
            const isWorkoutDeleted = check(deleteWorkoutRes, { '6. Deleted workout successfully': (r) => r.status === 200 });
            if (!isWorkoutDeleted) {
                //console.error(`[CHECK FAILED] 6. Delete Workout Failed: VU=${__VU}, Status=${deleteWorkoutRes.status}, Body=${deleteWorkoutRes.body}`);
            }
            sleep(1);
        }
    });
}


// --- Scenario 2: Browsing User ---
export function browsingFlow(data) {
    if (!data.userToken) return;
    const params = { headers: { 'Authorization': `Bearer ${data.userToken}` } };

    group('Browsing User: Read-Only Actions', function () {
        http.get(`${BASE_URL}/api/Category/GetAll`, params);
        sleep(getRandomInt(1, 3));

        const exercisesRes = http.get(`${BASE_URL}/api/Exercise/GetAll`, params);
        if (exercisesRes.body) {
            const exercisesBody = exercisesRes.json();
            if (check(exercisesRes, { 'Got all exercises': (r) => r.status === 200 && exercisesBody && exercisesBody.Result })) {
                const exercises = exercisesBody.Data;
                if (exercises && exercises.length > 0) {
                    const randomExercise = exercises[getRandomInt(0, exercises.length - 1)];
                    http.get(`${BASE_URL}/api/Exercise/GetById?Id=${randomExercise.id}`, params);
                }
            }
        }
        sleep(getRandomInt(1, 3));
    });
}


// --- Scenario 3: Admin User ---
export function adminFlow(data) {
    if (!data.adminToken) return;

    // Set headers for all requests in this flow.
    // Content-Type is crucial for the POST request to send a JSON body.
    const params = {
        headers: {
            'Authorization': `Bearer ${data.adminToken}`,
            'Content-Type': 'application/json'
        }
    };

    // Simpler params for requests that don't send a body
    const authHeaderOnly = { headers: { 'Authorization': `Bearer ${data.adminToken}` } };

    group('Admin User: Management Actions', function () {
        const getUsersRes = http.get(`${BASE_URL}/api/User/GetAllUsers/all`, authHeaderOnly);
        check(getUsersRes, { 'Admin got all users': (r) => r.status === 200 });
        sleep(1);

        let newCategoryId = null;

        const createCategoryUrl = `${BASE_URL}/api/Category/Insert`;
        const createPayload = JSON.stringify({
            Name: `New Category ${__VU}-${__ITER}`,
            Type: 1
        });
        const createRes = http.post(createCategoryUrl, createPayload, params);
        let createBody;
        if (createRes.body) { try { createBody = createRes.json(); } catch (e) { createBody = null; } }

        const isCategoryCreated = check(createRes, { 'Admin created category': (r) => r.status === 200 && createBody && createBody.Result });
        if (!isCategoryCreated) {
            //console.error(`[DEBUG] Category Creation Failed: VU=${__VU}, Status=${createRes.status}, Body=${createRes.body}`);
        } else {
            if (createBody.Data && createBody.Data.id) {
                newCategoryId = createBody.Data.id;
            } else {
                //console.error(`[DEBUG] Category Created, but no ID returned in response body. Body=${createRes.body}`);
            }
        }
        sleep(1);

        if (newCategoryId) {
            // --- FIX 1: Changed Update to use a JSON Body ---
            const updateCategoryUrl = `${BASE_URL}/api/Category/Update`;
            const updatePayload = JSON.stringify({
                Id: newCategoryId,
                Name: `Updated Category ${__VU}-${__ITER}`,
                Type: 1
            });
            const updateRes = http.put(updateCategoryUrl, updatePayload, params);
            const isCategoryUpdated = check(updateRes, { 'Admin updated category': (r) => r.status === 200 });
            if (!isCategoryUpdated) {
                //console.error(`[DEBUG] Category Update Failed: VU=${__VU}, Status=${updateRes.status}, Body=${updateRes.body}`);
            }
            sleep(1);

            // --- FIX 2: Changed Delete to use a JSON Body ---
            const deleteCategoryUrl = `${BASE_URL}/api/Category/Delete`;
            const deletePayload = JSON.stringify({
                Id: newCategoryId
            });
            const deleteRes = http.del(deleteCategoryUrl, deletePayload, params);
            const isCategoryDeleted = check(deleteRes, { 'Admin deleted category': (r) => r.status === 200 });
            if (!isCategoryDeleted) {
                //console.error(`[DEBUG] Category Delete Failed: VU=${__VU}, Status=${deleteRes.status}, Body=${deleteRes.body}`);
            }
            sleep(1);
        }
    });
}
