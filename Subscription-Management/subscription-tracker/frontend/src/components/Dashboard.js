import React, { useCallback, useEffect, useState } from 'react';
import './Dashboard.css';
import ConfirmDialog from './ConfirmDialog';
import {
  addSubscription as createSubscription,
  deleteSubscription as removeSubscription,
  getSubscriptions
} from '../services/api';
// dashboard UI previous UI updated it
function Dashboard({ user, onLogout, onShowAnalytics, onShowSettings }) {
  const [subscriptions, setSubscriptions] = useState([]);
  const [formData, setFormData] = useState({
    name: '',
    cost: '',
    company: '',
    renewalDate: ''
  });
  const [loading, setLoading] = useState(true);
  const [submitError, setSubmitError] = useState('');

  // state for confirmation dialog when deleting a subscription
  const [confirmDialog, setConfirmDialog] = useState({
    isOpen: false,
    type: null,
    subscriptionId: null,
    subscriptionName: '',
    isDeleting: false
  });

  const fetchSubscriptions = useCallback(async () => {
    try {
      setLoading(true);
      setSubmitError('');
      const response = await getSubscriptions(user.user_id);
      setSubscriptions(
        (response.data || []).map((subscription) => ({
          id: subscription.subscription_id,
          name: subscription.company_name,
          cost: Number(subscription.renewal_cost || 0).toFixed(2),
          company: subscription.company_name,
          renewalDate: subscription.sub_end || ''
        }))
      );
    } catch (error) { // error if cant connect to database
      setSubmitError(
        error?.response?.data?.error || 'Unable to load subscriptions right now.'
      );
    } finally {
      setLoading(false);
    }
  }, [user?.user_id]);

  useEffect(() => {
    if (user?.user_id) {
      fetchSubscriptions();
    } else {
      setLoading(false);
    }
  }, [fetchSubscriptions, user?.user_id]);

  const handleFormChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleAddSubscription = async (e) => {
    e.preventDefault();
    const parsedCost = Number.parseFloat(formData.cost);

    if (!formData.name || !formData.cost || !formData.renewalDate) {
      setSubmitError('Please fill out the subscription name, monthly cost, and renewal date.');
      return;
    }

    if (!Number.isFinite(parsedCost) || parsedCost < 0) {
      setSubmitError('Monthly cost must be a valid amount greater than or equal to 0.');
      return;
    }

    try {
      setSubmitError('');
      await createSubscription({
        user_id: user.user_id,
        company_name: formData.company || formData.name,
        renewal_cost: parsedCost,
        sub_end: formData.renewalDate
      });

      await fetchSubscriptions();
      setFormData({
        name: '',
        cost: '',
        company: '',
        renewalDate: ''
      });
    } catch (error) {
      setSubmitError(
        error?.response?.data?.error || 'Unable to add subscription right now.' 
      );
    }
  };

  const handleDeleteSubscriptionClick = (id, name) => {
    setConfirmDialog({
      isOpen: true,
      type: 'subscription',
      subscriptionId: id,
      subscriptionName: name,
      isDeleting: false
    });
  };

  // Execute the actual delete after confirmation
  const confirmSubscriptionDelete = async () => {
    const { subscriptionId } = confirmDialog;
    
    setConfirmDialog(prev => ({ ...prev, isDeleting: true }));
    
    try {
      setSubmitError('');
      await removeSubscription(subscriptionId);
      setSubscriptions((prev) => prev.filter((sub) => sub.id !== subscriptionId));
    } catch (error) {
      setSubmitError(
        error?.response?.data?.error || 'Unable to delete subscription right now.'
      );
    } finally {
      setConfirmDialog({
        isOpen: false,
        type: null,
        subscriptionId: null,
        subscriptionName: '',
        isDeleting: false
      });
    }
  };

  const handleConfirm = () => {
    confirmSubscriptionDelete();
  };

  const closeConfirmDialog = () => {
    setConfirmDialog(prev => ({ ...prev, isOpen: false }));
  };

  const totalMonthlySpend = subscriptions.reduce(
    (total, sub) => total + (Number.parseFloat(sub.cost) || 0),
    0
  );

  const averageMonthlySpend = subscriptions.length
    ? totalMonthlySpend / subscriptions.length
    : 0;

  return (
    <div className="dashboard-page">
  <div className="dashboard-shell">
   <header className="dashboard-header">
    <div>
         <h1>Subscription Dashboard</h1>
            <p className="dashboard-subtitle">
              {user?.email || 'Manage your subscriptions in one place.'}
           </p>
          </div>
    <div className="dashboard-actions">
            <span className="dashboard-greeting">
         {user?.name ? `Hi, ${user.name}` : 'Hi, User'}
      </span>
            <button
              type="button"
              className="secondary-button"
              onClick={onShowSettings}
            >
              Settings
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

    <section className="dashboard-banner">
    <h2>Keep every subscription organized.</h2>
    <p>
       Track what you pay each month spot overlaps, and jump into   
      analytics when you want the bigger picture.
          </p>
        </section>

        <section className="dashboard-metrics">
      <div className="dashboard-stat-card">
    <span className="dashboard-stat-label">Active Subscriptions</span>
    <strong className="dashboard-stat-value">
        {subscriptions.length}
       </strong>
    </div>
    <div className="dashboard-stat-card">
     <span className="dashboard-stat-label">Monthly Spend</span>
      <strong className="dashboard-stat-value">
       ${totalMonthlySpend.toFixed(2)}
     </strong>
       </div>
        <div className="dashboard-stat-card">
     <span className="dashboard-stat-label">Average Cost</span>
    <strong className="dashboard-stat-value">
      ${averageMonthlySpend.toFixed(2)}
            </strong>
        </div>
      </section>

  <section className="dashboard-card">
  <div className="dashboard-section-header dashboard-section-header--centered">
      <div>
      <h3>Subscriptions</h3>
         <p>
             Review your current list and remove anything you no longer
                need.
              </p>
            </div>
          </div>

  {submitError && (
      <div className="dashboard-message error-message">
        {submitError}
         </div>
          )}

  <div className="table-wrapper">
     <table className="dashboard-table">
         <thead>
     <tr>
       <th>ID</th>
     <th>Name</th>
     <th>Cost</th>
     <th>Renews On</th>
       <th>Company</th>
         <th>Action</th>
          </tr>
              </thead>
              <tbody>
                {loading && (
                  <tr>
                    <td colSpan="6" className="empty-state">
                      Loading subscriptions...
                    </td>
                  </tr>
                )}
        {subscriptions.map((sub) => (
               <tr key={sub.id}>
                    <td>{sub.id}</td>
                    <td>{sub.name}</td>
                    <td>${sub.cost}</td>
                    <td>{sub.renewalDate || '-'}</td>
                    <td>{sub.company || '-'}</td>
            <td>
              <button
             type="button"
            className="delete-btn"
              onClick={() => handleDeleteSubscriptionClick(sub.id, sub.name)}
                  >
               Delete
                      </button>
                    </td>
                  </tr>
                ))}
                {!loading && subscriptions.length === 0 && (
                  <tr>
                    <td colSpan="6" className="empty-state">
                      No subscriptions yet.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
          <div className="table-actions">
            <button
              type="button"
              className="secondary-button"
              onClick={onShowAnalytics}
            >
              View Analytics
            </button>
            <button
              type="button"
              className="primary-button"
              onClick={() => document.getElementById('add-subscription-form')?.scrollIntoView({ behavior: 'smooth' })}
            >
              Add Subscription
            </button>
          </div>
        </section>

        <section className="dashboard-card form-card">
          <div className="dashboard-section-header">
            <div>
              <h3>Add Subscription</h3>
              <p>Add a service to keep your dashboard up to date.</p>
            </div>
            <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={closeConfirmDialog}
        onConfirm={handleConfirm}
        title={confirmDialog.type === 'account' ? 'Delete Account' : 'Delete Subscription'}
        message={
          confirmDialog.type === 'account' 
            ? `Are you sure you want to permanently delete your account? This will remove ALL your subscriptions and data. This action CANNOT be undone!`
            : `Are you sure you want to delete "${confirmDialog.subscriptionName}"? This action cannot be undone.`
        }
        confirmText={confirmDialog.isDeleting ? 'Deleting...' : (confirmDialog.type === 'account' ? 'Delete Account' : 'Delete')}
        cancelText="Cancel"
        confirmButtonClass="confirm-danger"
      />
          </div>

          <form id="add-subscription-form" onSubmit={handleAddSubscription}>
            <input
              name="name"
              value={formData.name}
              onChange={handleFormChange}
              placeholder="Subscription name"
            />
            <input
              name="cost"
              type="number"
              min="0"
              step="0.01"
              value={formData.cost}
              onChange={handleFormChange}
              placeholder="Monthly cost"
            />
            <input
              name="renewalDate"
              type="date"
              value={formData.renewalDate}
              onChange={handleFormChange}
              placeholder="Select renewal date"
            />
            <small className="help-text">Date the subscription will renew</small>
            <input
              name="company"
              value={formData.company}
              onChange={handleFormChange}
              placeholder="Company"
            />
            <button type="submit" className="primary-button">
              Add Subscription
            </button>
          </form>
        </section>
      </div>
      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={closeConfirmDialog}
        onConfirm={handleConfirm}
        title={confirmDialog.type === 'account' ? 'Delete Account' : 'Delete Subscription'}
        message={
          confirmDialog.type === 'account' 
            ? `Are you sure you want to permanently delete your account? This will remove ALL your subscriptions and data. This action CANNOT be undone!`
            : `Are you sure you want to delete "${confirmDialog.subscriptionName}"? This action cannot be undone.`
        }
        confirmText={confirmDialog.isDeleting ? 'Deleting...' : (confirmDialog.type === 'account' ? 'Delete Account' : 'Delete')}
        cancelText="Cancel"
        confirmButtonClass="confirm-danger"
      />
    </div>
  );
}

export default Dashboard;
