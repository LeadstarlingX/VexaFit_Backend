import React, { useState, useEffect } from 'react';
import { BookCopy, PlusCircle, Edit, Trash2 } from 'lucide-react';
import { dataService } from '../../services/dataService.js';
import Modal from '../../components/Modal/Modal.jsx';
import './ExercisePage.css';
import '../UsersPage/UsersPage.css';


const ExerciseForm = ({ exercise, allCategories, onSave, onCancel }) => {
    const [name, setName] = useState(exercise?.Name || '');
    const [description, setDescription] = useState(exercise?.Description || '');
    const [selectedCategories, setSelectedCategories] = useState(exercise?.Categories?.map(c => c.Id) || []);
    const [imageFile, setImageFile] = useState(null);

    const handleSubmit = (e) => {
        e.preventDefault();
        onSave({
            ...exercise,
            Name: name,
            Description: description,
            CategoryIds: selectedCategories,
            ImageFile: imageFile
        });
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="input-group">
                <label htmlFor="exercise-name">Exercise Name</label>
                <input id="exercise-name" type="text" value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <div className="input-group">
                <label htmlFor="exercise-desc">Description</label>
                <textarea id="exercise-desc" value={description} onChange={(e) => setDescription(e.target.value)} required />
            </div>
            <div className="input-group">
                <label htmlFor="exercise-categories">Categories</label>
                <select
                    id="exercise-categories"
                    multiple
                    value={selectedCategories}
                    onChange={(e) => setSelectedCategories(Array.from(e.target.selectedOptions, option => parseInt(option.value)))}
                    className="multi-select"
                >
                    {allCategories.map(cat => (
                        <option key={cat.Id} value={cat.Id}>{cat.Name}</option>
                    ))}
                </select>
            </div>
            <div className="input-group">
                <label htmlFor="exercise-image">Image</label>
                <input id="exercise-image" type="file" onChange={(e) => setImageFile(e.target.files[0])} accept="image/*" />
            </div>
            <div className="form-actions">
                <button type="button" className="cancel-button" onClick={onCancel}>Cancel</button>
                <button type="submit" className="save-button">Save Exercise</button>
            </div>
        </form>
    );
};

const ExercisesPage = () => {
    const [exercises, setExercises] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingExercise, setEditingExercise] = useState(null);

    const fetchData = async () => {
        try {
            setLoading(true);
            const [exercisesData, categoriesData] = await Promise.all([
                dataService.getExercises(),
                dataService.getCategories()
            ]);
            setExercises(exercisesData);
            setCategories(categoriesData);
            setError(null);
        } catch (err) {
            setError("Failed to load data.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleAddClick = () => {
        setEditingExercise(null);
        setIsModalOpen(true);
    };

    const handleEditClick = (exercise) => {
        setEditingExercise(exercise);
        setIsModalOpen(true);
    };

    const handleDeleteClick = async (id) => {
        if (window.confirm('Are you sure you want to delete this exercise?')) {
            try {
                await dataService.deleteExercise(id);
                fetchData();
            } catch (err) {
                alert('Failed to delete exercise.');
                console.error(err);
            }
        }
    };

    const handleSave = async (exerciseData) => {
        try {
            if (editingExercise) {
                await dataService.updateExercise(exerciseData);
            } else {
                await dataService.createExercise(exerciseData);
            }
            setIsModalOpen(false);
            fetchData();
        } catch (err) {
            alert('Failed to save exercise.');
            console.error(err);
        }
    };

    if (loading) return <div className="page-content"><h2>Loading Exercises...</h2></div>;
    if (error) return <div className="page-content"><h2 style={{ color: 'red' }}>{error}</h2></div>;

    return (
        <div>
            <div className="page-header">
                <h2 className="page-title">Exercise Management</h2>
                <button className="add-button" onClick={handleAddClick}>
                    <PlusCircle size={18} className="add-button-icon" /> Add Exercise
                </button>
            </div>
            <div className="card">
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Image</th>
                                <th>Name</th>
                                <th>Categories</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {exercises.map(ex => (
                                <tr key={ex.Id}>
                                    <td>
                                        <img
                                            src={ex.Image?.[0]?.Url}
                                            alt={ex.Name}
                                            className="table-image-thumb"
                                            onError={(e) => e.target.src = 'https://placehold.co/100x100?text=No+Img'}
                                        />
                                    </td>
                                    <td>{ex.Name}</td>
                                    <td>{ex.Categories?.map(c => c.Name).join(', ') || 'N/A'}</td>
                                    <td>
                                        <button className="action-button" onClick={() => handleEditClick(ex)}>
                                            <Edit size={18} />
                                        </button>
                                        <button className="action-button danger" onClick={() => handleDeleteClick(ex.Id)}>
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
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                title={editingExercise ? 'Edit Exercise' : 'Add New Exercise'}
            >
                <ExerciseForm
                    exercise={editingExercise}
                    allCategories={categories}
                    onSave={handleSave}
                    onCancel={() => setIsModalOpen(false)}
                />
            </Modal>
        </div>
    );
};

export default ExercisesPage;
