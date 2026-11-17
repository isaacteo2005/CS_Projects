import React, { useState } from 'react';
import './Register.css'; 
//collects user info and creates account
function Register({onShowLogin}){
    // state for form fields
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    // State for error messages
    const [errors, setErrors] = useState({});
    // Shows registration successful message
    const [successMessage, setSuccessMessage] = useState('');
    // prevents double submissions while waiting for server
    const [isSubmitting, setIsSubmitting] = useState(false);

// event handlers

     // Handle input changes, runs everytime user types something into a field
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
        // Clear error for this field when user starts typing
        if (errors[name]) {
            setErrors({
                ...errors,
                [name]: ''
            });
        }
    };

     const validateForm = () => {
        const newErrors = {};
        
        // Name validation
        if (!formData.name.trim()) {
            newErrors.name = 'Name is required';
        } else if (formData.name.length < 2) {
            newErrors.name = 'Name must be at least 2 characters';
        }
        
        // Email validation
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!formData.email) {
            newErrors.email = 'Email is required';
        } else if (!emailRegex.test(formData.email)) {
            newErrors.email = 'Please enter a valid email address';
        }
        
        // Password validation
        if (!formData.password) {
            newErrors.password = 'Password is required';
        } else if (formData.password.length < 6) {
            newErrors.password = 'Password must be at least 6 characters';
        }
        
        // Confirm password validation
        if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = 'Passwords do not match';
        }
        
        return newErrors;
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        // stops page from refreshing
        e.preventDefault();
        
        // Validate form
        const newErrors = validateForm();
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }
        
        setIsSubmitting(true);
        setErrors({});
        setSuccessMessage('');
        
        try {
            // Send registration request to backend
            const response = await fetch('http://localhost:3001/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    name: formData.name,
                    email: formData.email,
                    password: formData.password
                })
            });
            
            const data = await response.json();
            
            if (response.ok) {
                // Registration successful
                setSuccessMessage('Registration successful! You can now log in.');
                // Clear form
                setFormData({
                    name: '',
                    email: '',
                    password: '',
                    confirmPassword: ''
                });
                console.log('User registered successfully:', data);
            } else {
                // Registration failed
                setErrors({ submit: data.error || 'Registration failed. Please try again.' });
            }
        } catch (error) {
            console.error('Registration error:', error);
            setErrors({ submit: 'Network error. Please check if the server is running.' });
        } finally {
            setIsSubmitting(false);
        }
    };

     return (
        <div className="register-container">
            <div className="register-card">
                <h2>Create Account</h2>
                <p className="subtitle">Join Subscription Tracker today</p>
                
                {successMessage && (
                    <div className="success-message">
                        {successMessage}
                    </div>
                )}
                
                {errors.submit && (
                    <div className="error-message">
                        {errors.submit}
                    </div>
                )}
                
                <form onSubmit={handleSubmit}>
                    {/* Name Field */}
                    <div className="form-group">
                        <label htmlFor="name">Full Name</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            placeholder="Enter your full name"
                            className={errors.name ? 'error' : ''}
                        />
                        {errors.name && (
                            <span className="field-error">{errors.name}</span>
                        )}
                    </div>
                    
                    {/* Email Field */}
                    <div className="form-group">
                        <label htmlFor="email">Email Address</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            placeholder="Enter your email"
                            className={errors.email ? 'error' : ''}
                        />
                        {errors.email && (
                            <span className="field-error">{errors.email}</span>
                        )}
                        <small className="hint">
                            Used for account recovery and 2FA
                        </small>
                    </div>
                    
                    {/* Password Field */}
                    <div className="form-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            placeholder="Create a password"
                            className={errors.password ? 'error' : ''}
                        />
                        {errors.password && (
                            <span className="field-error">{errors.password}</span>
                        )}
                        <small className="hint">
                            Must be at least 6 characters
                        </small>
                    </div>
                    
                    {/* Confirm Password Field */}
                    <div className="form-group">
                        <label htmlFor="confirmPassword">Confirm Password</label>
                        <input
                            type="password"
                            id="confirmPassword"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            placeholder="Confirm your password"
                            className={errors.confirmPassword ? 'error' : ''}
                        />
                        {errors.confirmPassword && (
                            <span className="field-error">{errors.confirmPassword}</span>
                        )}
                    </div>
                    
                    {/* Register Button */}
                    <button 
                        type="submit" 
                        className="register-button"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Creating Account...' : 'Register'}
                    </button>
                </form>
                
                <div className="login-link">
                    <p>
                        Already have an account?{' '}
                        <span 
                            className="link" 
                            onClick={() => {if (onShowLogin) {onShowLogin()}}}
                        >
                            Log in here
                        </span>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default Register;