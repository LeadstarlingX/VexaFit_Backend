import React, { useState, useEffect } from 'react';
import Modal from '../Modal/Modal.jsx';
import { dataService } from '../../services/dataService.js';
import { Trash2, PlusCircle } from 'lucide-react';
import './ManageWorkoutExercisesModal.css';

const ManageWorkoutExercisesModal = ({ workout, onClose, onUpdate }) => {
    const [allExercises, setAllExercises] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [filteredExercises, setFilteredExercises] = useState([]);

    useEffect(() => {
        // Fetch all exercises to make them available for adding
        const fetchAllExercises = async () => {
            const exercises = await dataService.getExercises();
            setAllExercises(exercises);
            setFilteredExercises(exercises);
        };
        fetchAllExercises();
    }, []);

    useEffect(() => {
        // ✨ UPDATED LOGIC
        // Get the IDs of exercises that are already in the current workout.
        const currentExerciseIds = workout?.WorkoutExercises?.map(we => we.Exercise.Id) || [];

        const lowercasedFilter = searchTerm.toLowerCase();

        const filtered = allExercises.filter(ex => {
            // An exercise is available if it meets two conditions:
            // 1. It matches the current search term.
            const matchesSearch = ex.Name.toLowerCase().includes(lowercasedFilter);
            // 2. It is NOT already in the current workout.
            const notAlreadyAdded = !currentExerciseIds.includes(ex.Id);

            return matchesSearch && notAlreadyAdded;
        });

        setFilteredExercises(filtered);
    }, [searchTerm, allExercises, workout]);

    const handleAddExercise = async (exerciseId) => {
        // Placeholder for sets and reps, you could add a small form for this
        const payload = {
            workoutId: workout.Id,
            exerciseId: exerciseId,
            sets: 3, // Default value
            reps: 10, // Default value
        };
        try {
            await dataService.addExerciseToWorkout(payload);
            onUpdate(); // Trigger a refresh of the workout list
        } catch (err) {
            alert("Failed to add exercise.");
            console.error(err);
        }
    };

    const handleRemoveExercise = async (workoutExerciseId) => {
        if (window.confirm('Are you sure you want to remove this exercise from the workout?')) {
            try {
                await dataService.removeExerciseFromWorkout(workoutExerciseId);
                onUpdate(); // Trigger a refresh
            } catch (err) {
                alert("Failed to remove exercise.");
                console.error(err);
            }
        }
    };

    return (
        <Modal isOpen={!!workout} onClose={onClose} title={`Manage Exercises for: ${workout?.Name}`}>
            <div className="manage-exercises-container">
                {/* Left side: Current exercises in the workout */}
                <div className="current-exercises-panel">
                    <h3>Current Exercises</h3>
                    <ul>
                        {workout?.WorkoutExercises?.length > 0 ? (
                            workout.WorkoutExercises.map(we => (
                                <li key={we.Id}>
                                    <span>{we.Exercise?.Name}</span>
                                    <button onClick={() => handleRemoveExercise(we.Id)}>
                                        <Trash2 size={16} />
                                    </button>
                                </li>
                            ))
                        ) : (
                            <p>No exercises in this workout yet.</p>
                        )}
                    </ul>
                </div>

                {/* Right side: List of all available exercises to add */}
                <div className="available-exercises-panel">
                    <h3>Available Exercises</h3>
                    <input
                        type="text"
                        placeholder="Search to add..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                    />
                    <ul>
                        {filteredExercises.map(ex => (
                            <li key={ex.Id}>
                                <span>{ex.Name}</span>
                                <button onClick={() => handleAddExercise(ex.Id)}>
                                    <PlusCircle size={16} />
                                </button>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </Modal>
    );
};

export default ManageWorkoutExercisesModal;
