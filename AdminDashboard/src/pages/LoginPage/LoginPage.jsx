import React, { useState } from 'react';
import './LoginPage.css';


const LoginPage = ({ handleLogin, error, loading }) => {
    const [email, setEmail] = useState('Admin@vexafit.com'); 
    const [password, setPassword] = useState('Admin123#'); 

    const handleSubmit = (e) => {
        e.preventDefault();
        handleLogin({ email, password });
    };

    return (
        <div className="login-container">
            <div className="login-card">
                <h1 className="login-title">VexaFit Admin</h1>
                <p className="login-subtitle">Please sign in to continue</p>
                <form onSubmit={handleSubmit}>
                    <div className="input-group">
                        <label htmlFor="email">Email</label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="input-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    {error && <p className="error-message">{error}</p>}
                    <button type="submit" className="login-button" disabled={loading}>
                        {loading ? 'Signing In...' : 'Sign In'}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;
