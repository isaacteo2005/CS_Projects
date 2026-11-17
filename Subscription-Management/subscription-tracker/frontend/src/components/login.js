/*
 * Handles user authentication UI:
 * - Email/password input form
 * - API call to /api/auth/login
 */
//collects email/password and sends them to backend
import React, { useState } from 'react';
import './Register.css';
import { login } from '../services/api';

function Login({ onLogin, onShowRegister }) {
    // Form values typed by the user
    const [formData, setFormData] = useState({
        email: '',
        password: ''
    });
    // Stores one general submit/login error
    const [submitError, setSubmitError] = useState('');

    // Stores success message
    const [successMessage, setSuccessMessage] = useState('');

    // True while waiting for login response
    const [isSubmitting, setIsSubmitting] = useState(false);

    // Update the input field that changed
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });

    };

    // Runs when the user clicks Login
    const handleSubmit = async (e) => {
        // Prevent page reload
        e.preventDefault();

        // Start loading state and clear old submit errors
        setIsSubmitting(true);
        setSubmitError('');
        setSuccessMessage('');

        try {
            // Send login request to backend
            const response = await login({
                email: formData.email,
                password: formData.password
            });
            console.log('Login response:', response);
            // On success, pass user data to parent component
            if (response?.data?.success) {
                const userName = response.data.user?.name || 'User';
                setSuccessMessage(`✅ Login successful! Welcome back, ${userName}!`);
                setFormData({ email: '', password: '' }); // Clear form fields
                if (onLogin) {
                    onLogin(response.data.user);
                }
            } else {
                // Backend rejected credentials
                setSubmitError(response?.data?.error || 'Incorrect email or password');
            }
        } catch (error) {
            // Handle network/server/login errors
            const message =
                error?.response?.data?.error || error?.response?.data?.message ||
                'Incorrect email or password';
            setSubmitError(message);
        } finally {
            // End loading state no matter what happened
            setIsSubmitting(false);
        }
    };

    return (
        // Main page wrapper
        <div className="register-container">
            {/* Centered login card */}
            <div className="register-card">

                <h2>Welcome Back</h2>
                <p className="subtitle">Log in to manage your subscriptions</p>
                 {/* SUCCESS MESSAGE - Put it HERE */}
            {successMessage && (
                <div style={{
                    backgroundColor: '#d4edda',
                    color: '#155724',
                    padding: '15px',
                    borderRadius: '6px',
                    marginBottom: '20px',
                    textAlign: 'center',
                    border: '1px solid #c3e6cb',
                    fontWeight: '500',
                    fontSize: '16px'
                }}>
                    {successMessage}
                </div>
            )}

                {submitError && (
                    // Top-level login error (wrong credentials, server issues, etc.)
                    <div className="error-message">
                        {submitError}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    {/* Email field */}
                    <div className="form-group">
                        <label htmlFor="email">Email Address</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            placeholder="Enter your email"
                        />
                    </div>

                    {/* Password field */}
                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder="Enter your password"
                        />
                    </div>

                    {/* Submit button */}
                    <button
                        type="submit"
                        className="register-button"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Logging in...' : 'Login'}
                    </button>
                </form>

                {/* Placeholder link to registration page (not wired yet) */}
                <div className="login-link">
                    <p>
                        Do not have an account?{' '}
                        <span
                            className="link"
                            onClick={() => {
                                if (onShowRegister) {
                                    onShowRegister();
                                }
                            }}
                        >
                            Sign up here
                        </span>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default Login;
