import React, { useState } from 'react';
import { Users, Dumbbell, Activity, LogOut, Settings, X, ChevronDown, ChevronUp, BookCopy, LayoutGrid } from 'lucide-react';
import './Sidebar.css';

const Sidebar = ({ isOpen, setIsOpen, currentPage, setCurrentPage, handleLogout }) => {
    // ✨ NEW: State to manage the collapsible content menu
    const [isContentMenuOpen, setIsContentMenuOpen] = useState(true);

    const mainNavItems = [
        { name: 'Dashboard', icon: Activity, page: 'dashboard' },
        { name: 'Users', icon: Users, page: 'users' },
    ];

    const contentNavItems = [
        { name: 'Workouts', icon: Dumbbell, page: 'workouts' },
        { name: 'Exercises', icon: BookCopy, page: 'exercises' },
        { name: 'Categories', icon: LayoutGrid, page: 'categories' },
    ];

    const handleNavigation = (page) => {
        setCurrentPage(page);
        if (isOpen) setIsOpen(false);
    };

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
                    {/* Main Navigation */}
                    {mainNavItems.map((item) => (
                        <a
                            key={item.name}
                            href="#"
                            onClick={(e) => {
                                e.preventDefault();
                                handleNavigation(item.page);
                            }}
                            className={`nav-item ${currentPage === item.page ? 'active' : ''}`}
                        >
                            <item.icon className="nav-item-icon" size={22} />
                            {item.name}
                        </a>
                    ))}

                    {/* ✨ NEW: Collapsible Content Management Section */}
                    <div className="nav-section">
                        <button
                            className="nav-section-header"
                            onClick={() => setIsContentMenuOpen(!isContentMenuOpen)}
                        >
                            <span>Content Management</span>
                            {isContentMenuOpen ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
                        </button>
                        {isContentMenuOpen && (
                            <div className="nav-submenu">
                                {contentNavItems.map((item) => (
                                    <a
                                        key={item.name}
                                        href="#"
                                        onClick={(e) => {
                                            e.preventDefault();
                                            handleNavigation(item.page);
                                        }}
                                        className={`nav-item nav-sub-item ${currentPage === item.page ? 'active' : ''}`}
                                    >
                                        <item.icon className="nav-item-icon" size={20} />
                                        {item.name}
                                    </a>
                                ))}
                            </div>
                        )}
                    </div>
                </nav>

                {/* Footer Navigation (Settings & Logout) */}
                <div className="sidebar-footer">
                    <a href="#" onClick={(e) => { e.preventDefault(); handleNavigation('settings'); }} className={`nav-item ${currentPage === 'settings' ? 'active' : ''}`}>
                        <Settings className="nav-item-icon" size={22} />
                        Settings
                    </a>
                    <a href="#" onClick={handleLogout} className="nav-item">
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
