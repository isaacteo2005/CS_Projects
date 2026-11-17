import React, { useState, useEffect, useCallback } from 'react';
import './Analytics.css';
// this is the source code for Analytics page
function Analytics({ user, onLogout, onBack }) {
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [debugInfo, setDebugInfo] = useState('');

    const fetchUserStats = useCallback(async () => {
        try {
            setLoading(true);
            setError('');
            setDebugInfo('Fetching stats...');
            
            console.log('1. Fetching stats for user ID:', user.user_id);
            
            const statsResponse = await fetch(`http://localhost:3001/api/user-stats/${user.user_id}`);
            
            console.log('2. Response status:', statsResponse.status);
            setDebugInfo(`Status: ${statsResponse.status}`);
            
            if (!statsResponse.ok) {
                const errorText = await statsResponse.text();
                console.log('3. Error response:', errorText);
                throw new Error(`Failed to fetch statistics: ${statsResponse.status}`);
            }
            
            const statsData = await statsResponse.json();
            console.log('4. Stats data received:', statsData);
            setDebugInfo('Stats loaded successfully');
            setStats(statsData);
            
        } catch (err) {
            console.error('5. Analytics fetch error:', err);
            setError(err.message);
            setDebugInfo(`Error: ${err.message}`);
        } finally {
            setLoading(false);
        }
    }, [user]);

    useEffect(() => {
        if (user?.user_id) {
            fetchUserStats();
        } else {
            setError('No user logged in');
            setLoading(false);
        }
    }, [fetchUserStats, user?.user_id]);

    // Format currency
    const formatCurrency = (amount) => {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }).format(amount || 0);
    };

    // Format date
    const formatDate = (dateString) => {
        if (!dateString) return 'N/A';
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    };

    if (loading) {
        return (
            <div className="analytics-container">
                <div className="analytics-card">
                    <div className="loading-spinner">
                        <div className="spinner"></div>
                        <p>Loading your analytics...</p>
                        {debugInfo && <small>{debugInfo}</small>}
                    </div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="analytics-container">
                <div className="analytics-card">
                    <div className="error-container">
                        <div className="error-icon">⚠️</div>
                        <h2>Unable to Load Analytics</h2>
                        <p className="error-message">{error}</p>
                        {debugInfo && <small className="debug-info">{debugInfo}</small>}
                        <button onClick={fetchUserStats} className="retry-button">
                            Try Again
                        </button>
                        <button onClick={onLogout} className="logout-button" style={{marginTop: '10px'}}>
                            Back to Login
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="analytics-container">
            <div className="analytics-card">
                <div className="analytics-header">
                    <div>
                        <h1>Your Subscription Analytics</h1>
                        <p className="user-email">{user?.email}</p>
                        <small className="user-id">User ID: {user?.user_id}</small>
                    </div>
                    <div style={{ display: 'flex', gap: '10px' }}>
                        {onBack && (
                            <button onClick={onBack} className="refresh-button">
                                Back to Dashboard
                            </button>
                        )}
                        <button onClick={onLogout} className="logout-button">
                            Logout
                        </button>
                    </div>
                </div>

                <div className="welcome-banner">
                    <h2>Welcome back, {user?.name}!</h2>
                    <p>Here's a detailed look at your subscription spending and activity.</p>
                </div>

                <div className="metrics-grid">
                    <div className="metric-card primary">
                        <div className="metric-icon">💰</div>
                        <div className="metric-content">
                            <div className="metric-label">Total Spent</div>
                            <div className="metric-value">{formatCurrency(stats?.overall_spent)}</div>
                            <div className="metric-trend">Lifetime total</div>
                        </div>
                    </div>

                    <div className="metric-card info">
                        <div className="metric-icon">📊</div>
                        <div className="metric-content">
                            <div className="metric-label">Total Logins</div>
                            <div className="metric-value">{stats?.session_count || 0}</div>
                            <div className="metric-trend">Session count</div>
                        </div>
                    </div>

                    <div className="metric-card success">
                        <div className="metric-icon">🕐</div>
                        <div className="metric-content">
                            <div className="metric-label">Last Login</div>
                            <div className="metric-value">{formatDate(stats?.last_login)}</div>
                            <div className="metric-trend">Most recent visit</div>
                        </div>
                    </div>

                    <div className="metric-card accent">
                        <div className="metric-icon">📅</div>
                        <div className="metric-content">
                            <div className="metric-label">Member Since</div>
                            <div className="metric-value">{formatDate(user?.created_at)}</div>
                            <div className="metric-trend">Account created</div>
                        </div>
                    </div>
                </div>

                {stats?.most_spentname && (
                    <div className="highlight-card">
                        <div className="highlight-icon">🏆</div>
                        <div className="highlight-content">
                            <div className="highlight-label">Most Expensive Subscription</div>
                            <div className="highlight-title">{stats.most_spentname}</div>
                        </div>
                    </div>
                )}

                {stats?.last_deletedname && (
                    <div className="deleted-card">
                        <div className="deleted-icon">🗑️</div>
                        <div className="deleted-content">
                            <div className="deleted-label">Recently Deleted</div>
                            <div className="deleted-title">{stats.last_deletedname}</div>
                        </div>
                    </div>
                )}

                <div className="analytics-footer">
                    <button onClick={fetchUserStats} className="refresh-button">
                        🔄 Refresh Data
                    </button>
                </div>
            </div>
        </div>
    );
}

export default Analytics;
