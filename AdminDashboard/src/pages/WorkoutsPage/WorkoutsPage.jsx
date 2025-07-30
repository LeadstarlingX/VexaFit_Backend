import React, { useState, useEffect } from 'react';
import { Dumbbell, PlusCircle, Edit, Trash2, SlidersHorizontal } from 'lucide-react';
import { dataService } from '../../services/dataService.js';
import Modal from '../../components/Modal/Modal.jsx';
import ManageWorkoutExercisesModal from '../../components/ManageWorkoutExercisesModal/ManageWorkoutExercisesModal.jsx';
import './WorkoutsPage.css';
import '../UsersPage/UsersPage.css';

const WorkoutForm = ({ workout, onSave, onCancel }) => {
    const [name, setName] = useState(workout?.Name || '');
    const [description, setDescription] = useState(workout?.Description || '');

    const handleSubmit = (e) => {
        e.preventDefault();
        onSave({ ...workout, Name: name, Description: description });
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="input-group">
                <label htmlFor="workout-name">Workout Name</label>
                <input id="workout-name" type="text" value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="input-group">
                <label htmlFor="workout-desc">Description</label>
                <textarea id="workout-desc" value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>
            <div className="form-actions">
                <button type="button" className="cancel-button" onClick={onCancel}>Cancel</button>
                <button type="submit" className="save-button">Save Workout</button>
            </div>
        </form>
    );
};

const WorkoutsPage = () => {
    const [workouts, setWorkouts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isFormModalOpen, setIsFormModalOpen] = useState(false);
    const [editingWorkout, setEditingWorkout] = useState(null);

    // ✨ NEW: State for the manage exercises modal
    const [managingWorkout, setManagingWorkout] = useState(null);

    const fetchWorkouts = async () => {
        try {
            setLoading(true);
            const data = await dataService.getWorkouts();
            setWorkouts(data);
            setError(null);
        } catch (err) {
            setError("Failed to load workouts.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchWorkouts();
    }, []);

    const handleAddClick = () => {
        setEditingWorkout(null);
        setIsFormModalOpen(true);
    };

    const handleEditClick = (workout) => {
        setEditingWorkout(workout);
        setIsFormModalOpen(true);
    };

    const handleDeleteClick = async (id) => {
        if (window.confirm('Are you sure you want to delete this workout?')) {
            try {
                await dataService.deleteWorkout(id);
                fetchWorkouts();
            } catch (err) {
                alert('Failed to delete workout.');
                console.error(err);
            }
        }
    };

    const handleSave = async (workoutData) => {
        try {
            if (editingWorkout) {
                await dataService.updateWorkout(workoutData);
            } else {
                await dataService.createWorkout(workoutData);
            }
            setIsFormModalOpen(false);
            fetchWorkouts();
        } catch (err) {
            alert('Failed to save workout.');
            console.error(err);
        }
    };

    if (loading) return <div className="page-content"><h2>Loading Workouts...</h2></div>;
    if (error) return <div className="page-content"><h2 style={{ color: 'red' }}>{error}</h2></div>;

    return (
        <div>
            <div className="page-header">
                <h2 className="page-title">Workout Management</h2>
                <button className="add-button" onClick={handleAddClick}>
                    <PlusCircle size={18} className="add-button-icon" /> Add Workout
                </button>
            </div>
            <div className="card">
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Description</th>
                                <th># of Exercises</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {workouts.map(wo => (
                                <tr key={wo.Id}>
                                    <td>{wo.Name}</td>
                                    <td className="description-cell">{wo.Description}</td>
                                    <td>{wo.WorkoutExercises?.length || 0}</td>
                                    <td>
                                        {/* ✨ NEW: Manage Exercises Button */}
                                        <button className="action-button" title="Manage Exercises" onClick={() => setManagingWorkout(wo)}>
                                            <SlidersHorizontal size={18} />
                                        </button>
                                        <button className="action-button" title="Edit Workout" onClick={() => handleEditClick(wo)}>
                                            <Edit size={18} />
                                        </button>
                                        <button className="action-button danger" title="Delete Workout" onClick={() => handleDeleteClick(wo.Id)}>
                                            <Trash2 size={18} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            <Modal
                isOpen={isFormModalOpen}
                onClose={() => setIsFormModalOpen(false)}
                title={editingWorkout ? 'Edit Workout' : 'Add New Workout'}
            >
                <WorkoutForm
                    workout={editingWorkout}
                    onSave={handleSave}
                    onCancel={() => setIsFormModalOpen(false)}
                />
            </Modal>

            {/* ✨ NEW: Render the Manage Exercises Modal */}
            {managingWorkout && (
                <ManageWorkoutExercisesModal
                    workout={managingWorkout}
                    onClose={() => setManagingWorkout(null)}
                    onUpdate={ async () => {
                        // Refresh the list to show updated exercise counts
                        await fetchWorkouts();

                        // Then, fetch the single, updated workout to refresh the modal's internal state
                        const updatedWorkout = await dataService.getWorkoutById(managingWorkout.Id);
                        setManagingWorkout(updatedWorkout);
                    }}
                />
            )}
        </div>
    );
};

export default WorkoutsPage;
