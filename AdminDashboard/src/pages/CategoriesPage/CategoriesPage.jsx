import React, { useState, useEffect } from 'react';
import { PlusCircle, Edit, Trash2 } from 'lucide-react';
import { dataService } from '../../services/dataService.js';
import Modal from '../../components/Modal/Modal.jsx';
import './CategoriesPage.css';
import '../UsersPage/UsersPage.css'; // Re-using some styles

// Enum for Category Types, matching your backend
const CategoryTypeEnum = {
    MuscleGroup: 0,
    ExerciseType: 1,
    Position: 2,
};

// A dedicated form component for adding/editing categories
const CategoryForm = ({ category, onSave, onCancel }) => {
    const [name, setName] = useState(category?.Name || '');
    const [type, setType] = useState(category?.Type ?? CategoryTypeEnum.MuscleGroup);

    const handleSubmit = (e) => {
        e.preventDefault();
        // The 'Id' is included for updates, and ignored for creates
        onSave({ ...category, Name: name, Type: type });
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="input-group">
                <label htmlFor="category-name">Category Name</label>
                <input
                    id="category-name"
                    type="text"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                />
            </div>
            <div className="input-group">
                <label htmlFor="category-type">Category Type</label>
                <select
                    id="category-type"
                    value={type}
                    onChange={(e) => setType(parseInt(e.target.value))}
                >
                    <option value={CategoryTypeEnum.MuscleGroup}>Muscle Group</option>
                    <option value={CategoryTypeEnum.ExerciseType}>Exercise Type</option>
                    <option value={CategoryTypeEnum.Position}>Position</option>
                </select>
            </div>
            <div className="form-actions">
                <button type="button" className="cancel-button" onClick={onCancel}>Cancel</button>
                <button type="submit" className="save-button">Save Category</button>
            </div>
        </form>
    );
};


const CategoriesPage = () => {
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingCategory, setEditingCategory] = useState(null);

    // Function to fetch or re-fetch categories
    const fetchCategories = async () => {
        try {
            setLoading(true);
            const data = await dataService.getCategories();
            setCategories(data);
            setError(null);
        } catch (err) {
            setError("Failed to load categories.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCategories();
    }, []);

    const handleAddClick = () => {
        setEditingCategory(null); // Ensure we're in "add" mode
        setIsModalOpen(true);
    };

    const handleEditClick = (category) => {
        setEditingCategory(category);
        setIsModalOpen(true);
    };

    const handleDeleteClick = async (id) => {
        if (window.confirm('Are you sure you want to delete this category?')) {
            try {
                await dataService.deleteCategory(id);
                fetchCategories(); // Refresh list after deleting
            } catch (err) {
                alert('Failed to delete category.');
                console.error(err);
            }
        }
    };

    const handleSave = async (categoryData) => {
        try {
            if (editingCategory) {
                await dataService.updateCategory(categoryData);
            } else {
                await dataService.createCategory(categoryData);
            }
            setIsModalOpen(false);
            fetchCategories(); // Refresh list after saving
        } catch (err) {
            alert('Failed to save category.');
            console.error(err);
        }
    };

    if (loading) return <div className="page-content"><h2>Loading Categories...</h2></div>;
    if (error) return <div className="page-content"><h2 style={{ color: 'red' }}>{error}</h2></div>;

    return (
        <div>
            <div className="page-header">
                <h2 className="page-title">Category Management</h2>
                <button className="add-button" onClick={handleAddClick}>
                    <PlusCircle size={18} className="add-button-icon" /> Add Category
                </button>
            </div>
            <div className="card">
                <div className="table-container">
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Type</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {categories.map(cat => (
                                <tr key={cat.Id}>
                                    <td>{cat.Name}</td>
                                    <td>{Object.keys(CategoryTypeEnum).find(key => CategoryTypeEnum[key] === cat.Type)}</td>
                                    <td>
                                        <button className="action-button" onClick={() => handleEditClick(cat)}>
                                            <Edit size={18} />
                                        </button>
                                        <button className="action-button danger" onClick={() => handleDeleteClick(cat.Id)}>
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
                title={editingCategory ? 'Edit Category' : 'Add New Category'}
            >
                <CategoryForm
                    category={editingCategory}
                    onSave={handleSave}
                    onCancel={() => setIsModalOpen(false)}
                />
            </Modal>
        </div>
    );
};

export default CategoriesPage;
