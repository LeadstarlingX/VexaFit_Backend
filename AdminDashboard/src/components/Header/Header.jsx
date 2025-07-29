import React from 'react';
import { Menu, Search } from 'lucide-react';
import './Header.css';

const Header = ({ setIsOpen }) => {
    return (
        <header className="header">
            <button onClick={() => setIsOpen(true)} className="mobile-menu-btn">
                <Menu size={24} />
            </button>
            <div className="search-container">
                <input
                    type="text"
                    placeholder="Search..."
                    className="search-input"
                />
                <Search className="search-icon" size={18} />
            </div>
            <div className="admin-profile">
                <img
                    src="https://placehold.co/40x40/0088FE/FFFFFF?text=A"
                    alt="Admin"
                />
            </div>
        </header>
    );
};

export default Header;
