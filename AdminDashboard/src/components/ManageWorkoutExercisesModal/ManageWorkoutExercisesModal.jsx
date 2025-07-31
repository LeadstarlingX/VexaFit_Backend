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
        
        const fetchAllExercises = async () => {
            const exercises = await dataService.getExercises();
            setAllExercises(exercises);
            setFilteredExercises(exercises);
        };
        fetchAllExercises();
    }, []);

    useEffect(() => {
        const currentExerciseIds = workout?.WorkoutExercises?.map(we => we.Exercise.Id) || [];

        const lowercasedFilter = searchTerm.toLowerCase();

        const filtered = allExercises.filter(ex => {
            const matchesSearch = ex.Name.toLowerCase().includes(lowercasedFilter);
            const notAlreadyAdded = !currentExerciseIds.includes(ex.Id);

            return matchesSearch && notAlreadyAdded;
        });

        setFilteredExercises(filtered);
    }, [searchTerm, allExercises, workout]);

    const handleAddExercise = async (exerciseId) => {
        const payload = {
            workoutId: workout.Id,
            exerciseId: exerciseId,
            sets: 3, 
            reps: 10, 
        };
        try {
            await dataService.addExerciseToWorkout(payload);
            onUpdate(); 
        } catch (err) {
            alert("Failed to add exercise.");
            console.error(err);
        }
    };

    const handleRemoveExercise = async (workoutExerciseId) => {
        if (window.confirm('Are you sure you want to remove this exercise from the workout?')) {
            try {
                await dataService.removeExerciseFromWorkout(workoutExerciseId);
                onUpdate();
            } catch (err) {
                alert("Failed to remove exercise.");
                console.error(err);
            }
        }
    };

    return (
        <Modal isOpen={!!workout} onClose={onClose} title={`Manage Exercises for: ${workout?.Name}`}>
            <div className="manage-exercises-container">
                
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
