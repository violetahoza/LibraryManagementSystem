/**
 * Library Management System API Client
 * This file contains functions to interact with the backend API
 */

// API base URL
const API_BASE_URL = '/api';

// Book API functions
const BookAPI = {
    // Get all books
    getAllBooks: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/books`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching books:', error);
            return [];
        }
    },
    
    // Get a book by ID
    getBookById: async function(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/books/${id}`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error(`Error fetching book ${id}:`, error);
            return null;
        }
    },
    
    // Add a new book
    addBook: async function(bookData) {
        try {
            const response = await fetch(`${API_BASE_URL}/books`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(bookData)
            });
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error('Error adding book:', error);
            throw error;
        }
    },
    
    // Update a book
    updateBook: async function(bookData) {
        try {
            const response = await fetch(`${API_BASE_URL}/books/${bookData.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(bookData)
            });
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error(`Error updating book ${bookData.id}:`, error);
            throw error;
        }
    },
    
    // Delete a book
    deleteBook: async function(id) {
        try {
            const response = await fetch(`${API_BASE_URL}/books/${id}`, {
                method: 'DELETE'
            });
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return true;
        } catch (error) {
            console.error(`Error deleting book ${id}:`, error);
            throw error;
        }
    },
    
    // Search books
    searchBooks: async function(term, by = 'all') {
        try {
            const response = await fetch(`${API_BASE_URL}/books/search?term=${encodeURIComponent(term)}&by=${encodeURIComponent(by)}`);
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error(`Error searching books:`, error);
            return [];
        }
    },
    
    // Get popular books
    getPopularBooks: async function(count = 5) {
        try {
            const response = await fetch(`${API_BASE_URL}/books/popular?count=${count}`);
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error('Error fetching popular books:', error);
            return [];
        }
    },
    
    // Get related books
    getRelatedBooks: async function(bookId) {
        try {
            const response = await fetch(`${API_BASE_URL}/books/related?id=${bookId}`);
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error(`Error fetching related books:`, error);
            return [];
        }
    }
};

// Loan API functions
const LoanAPI = {
    // Get all loans
    getAllLoans: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/loans`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching loans:', error);
            return [];
        }
    },
    
    // Get active loans
    getActiveLoans: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/loans/active`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching active loans:', error);
            return [];
        }
    },
    
    // Get overdue loans
    getOverdueLoans: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/loans/overdue`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching overdue loans:', error);
            return [];
        }
    },
    
    // Borrow a book
    borrowBook: async function(bookId, patronName, patronEmail, loanDays = 14) {
        try {
            const response = await fetch(`${API_BASE_URL}/loans/borrow`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    bookId: bookId,
                    patronName: patronName,
                    patronEmail: patronEmail,
                    loanDays: loanDays
                })
            });
            
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.error || `Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error(`Error borrowing book:`, error);
            throw error;
        }
    },
    
    // Return a book
    returnBook: async function(loanId) {
        try {
            const response = await fetch(`${API_BASE_URL}/loans/return`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    loanId: loanId
                })
            });
            
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.error || `Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error(`Error returning book:`, error);
            throw error;
        }
    }
};

// Notification API functions
const NotificationAPI = {
    // Get all notifications
    getNotifications: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/notifications`);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching notifications:', error);
            return [];
        }
    },
    
    // Send all notifications
    sendNotifications: async function() {
        try {
            const response = await fetch(`${API_BASE_URL}/notifications/send`, {
                method: 'POST'
            });
            
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error('Error sending notifications:', error);
            throw error;
        }
    }
};

// Helper functions
function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

// Format date for display
function formatDate(dateString) {
    if (!dateString) return 'Never';
    const date = new Date(dateString);
    return date.toLocaleDateString();
}