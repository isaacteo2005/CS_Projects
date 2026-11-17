
//App.js controls page switching, and each main feature like login, registration, 
//dashboard, and analytics is separated into its own componen
/*
The main flow is:

user opens app
login page shows first
after successful login, app stores the current user
app switches to dashboard
from dashboard, user can go to analytics or log out

*/
//central place for API requests
import React, { useState } from 'react';
import Login from './components/login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import Analytics from './components/Analytics';
import Settings from './components/Settings';
import './App.css';

function App() {
  const [currentPage, setCurrentPage] = useState('login');
  const [currentUser, setCurrentUser] = useState(null);

  const goToLogin = () => {
    setCurrentPage('login');
  };

  const goToRegister = () => {
    setCurrentPage('register');
  };

  const handleLoginSuccess = (user) => {
    setCurrentUser(user || null);
    setCurrentPage('dashboard');
  };

  const handleLogout = () => {
    setCurrentUser(null);
    setCurrentPage('login');
  };

  const goToDashboard = () => {
    if (currentUser) {
      setCurrentPage('dashboard');
    }
  };

  const goToAnalytics = () => {
    if (currentUser) {
      setCurrentPage('analytics');
    }
  };

  const goToSettings = () => {
    if (currentUser) {
      setCurrentPage('settings');
    }
  };

  return (
    <div className="App">
      {currentPage === 'login' && (
        <Login onShowRegister={goToRegister} onLogin={handleLoginSuccess} />
      )}
      {currentPage === 'register' && (
        <Register onShowLogin={goToLogin} />
      )}
      {currentPage === 'dashboard' && (
        <Dashboard
          user={currentUser}
          onLogout={handleLogout}
          onShowAnalytics={goToAnalytics}
          onShowSettings={goToSettings}
        />
      )}
      {currentPage === 'analytics' && (
        <Analytics
          user={currentUser}
          onLogout={handleLogout}
          onBack={goToDashboard}
        />
      )}
      {currentPage === 'settings' && (
        <Settings
          user={currentUser}
          onLogout={handleLogout}
          onBack={goToDashboard}
        />
      )}
    </div>
  );
}

export default App;
