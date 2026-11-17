import React, { useState } from 'react';
import './Dashboard.css';
import ConfirmDialog from './ConfirmDialog';
import { deleteAccount as removeAccount } from '../services/api';

function Settings({ user, onLogout, onBack }) {
  const [confirmDialog, setConfirmDialog] = useState({
    isOpen: false,
    isDeleting: false
  });
  const [submitError, setSubmitError] = useState('');

  const handleDeleteClick = () => {
    setConfirmDialog({ isOpen: true, isDeleting: false });
  };

  const handleDeleteAccount = async () => {
    setConfirmDialog((prev) => ({ ...prev, isDeleting: true }));
    setSubmitError('');

    try {
      await removeAccount(user.user_id);
      onLogout();
    } catch (error) {
      setSubmitError(
        error?.response?.data?.error || 'Unable to delete account right now.'
      );
      setConfirmDialog({ isOpen: false, isDeleting: false });
    }
  };

  const closeConfirmDialog = () => {
    setConfirmDialog((prev) => ({ ...prev, isOpen: false }));
  };

  return (
    <div className="dashboard-page">
      <div className="dashboard-shell">
        <header className="dashboard-header">
          <div>
            <h1>Settings</h1>
            <p className="dashboard-subtitle">
              Manage your account actions and security settings.
            </p>
          </div>
          <div className="dashboard-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={onBack}
            >
              Back
            </button>
            <button
              type="button"
              className="primary-button"
              onClick={onLogout}
            >
              Logout
            </button>
          </div>
        </header>

        <section className="dashboard-card settings-card">
          <div className="dashboard-section-header">
            <div>
              <h3>Account Settings</h3>
              <p>Delete your account and all your subscriptions.</p>
            </div>
          </div>

          {submitError && (
            <div className="dashboard-message error-message">
              {submitError}
            </div>
          )}

          <div className="settings-content">
            <p>
              This will permanently remove your account, subscription data,
              and user statistics. You will be logged out immediately.
            </p>
            <button
              type="button"
              className="delete-account-button"
              onClick={handleDeleteClick}
            >
              Delete Account
            </button>
          </div>
        </section>

        <ConfirmDialog
          isOpen={confirmDialog.isOpen}
          onClose={closeConfirmDialog}
          onConfirm={handleDeleteAccount}
          title="Delete Account"
          message="Are you sure you want to permanently delete your account? This cannot be undone."
          confirmText={confirmDialog.isDeleting ? 'Deleting...' : 'Delete Account'}
          cancelText="Cancel"
          confirmButtonClass="confirm-danger"
        />
      </div>
    </div>
  );
}

export default Settings;
