/*
Centralized API calls to backend:
 * login()            - Sends credentials to /api/auth/login
 * register()         - Sends user data to /api/auth/register
 * getSubscriptions() - Fetches user's subscriptions
 * 
 * Base URL: http://localhost:3001/api
*/
import axios from 'axios';

const API = axios.create({
    baseURL: 'http://localhost:3001/api'
});

export const login = (credentials) => API.post('/auth/login', credentials);
export const register = (userData) => API.post('/auth/register', userData);
export const getSubscriptions = (userId) => API.get(`/subscriptions/user/${userId}`);
export const addSubscription = (subscriptionData) => API.post('/subscriptions', subscriptionData);
export const deleteSubscription = (subscriptionId) => API.delete(`/subscriptions/${subscriptionId}`);
export const registerUser = (userData) => API.post('/auth/register', userData);
export const loginUser = (credentials) => API.post('/auth/login', credentials);
export const deleteAccount = (userId) => API.delete(`/users/${userId}`);

// interceptors for adding tokens will be added later
API.interceptors.response.use(
    response => response,
    error => {
        console.error('API Error:', error.response?.data || error.message);
        return Promise.reject(error);
    }
);

export default API;
