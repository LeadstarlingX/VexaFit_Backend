import React from 'react';
import { Users, Dumbbell, Activity, LogOut, Settings, X } from 'lucide-react';
import './Sidebar.css';

const Sidebar = ({ isOpen, setIsOpen, currentPage, setCurrentPage }) => {
    const navItems = [
        { name: 'Dashboard', icon: Activity, page: 'dashboard' },
        { name: 'Users', icon: Users, page: 'users' },
        { name: 'Workouts', icon: Dumbbell, page: 'workouts' },
        { name: 'Settings', icon: Settings, page: 'settings' },
    ];

    return (
        <>
            <div className={`sidebar ${!isOpen ? 'closed' : ''}`}>
                <div className="sidebar-header">
                    <h1 className="sidebar-title">VexaFit Admin</h1>
                    <button onClick={() => setIsOpen(false)} className="mobile-close-btn">
                        <X size={24} />
                    </button>
                </div>
                <nav className="sidebar-nav">
                    {navItems.map((item) => (
                        <a
                            key={item.name}
                            href="#"
                            onClick={(e) => {
                                e.preventDefault();
                                setCurrentPage(item.page);
                                if (isOpen) setIsOpen(false);
                            }}
                            className={`nav-item ${currentPage === item.page ? 'active' : ''}`}
                        >
                            <item.icon className="nav-item-icon" size={22} />
                            {item.name}
                        </a>
                    ))}
                </nav>
                <div className="sidebar-footer">
                    <a href="#" className="nav-item">
                        <LogOut className="nav-item-icon" size={22} />
                        Logout
                    </a>
                </div>
            </div>
            {isOpen && <div onClick={() => setIsOpen(false)} className="sidebar-mobile-overlay"></div>}
        </>
    );
};

export default Sidebar;
