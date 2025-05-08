using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class NotificationService
    {
        private List<Notification> _pendingNotifications = new List<Notification>();

        public void AddOverdueNotification(string patronEmail, string bookTitle, DateTime dueDate)
        {
            var message = $"OVERDUE NOTICE: The book '{bookTitle}' was due on {dueDate.ToShortDateString()}. Please return it as soon as possible.";
            _pendingNotifications.Add(new Notification
            {
                PatronEmail = patronEmail,
                Message = message
            });
        }
        
        public void AddReservationNotification(string patronEmail, string bookTitle)
        {
            var message = $"RESERVATION NOTICE: The book '{bookTitle}' that you reserved is now available. You can pick it up within the next 3 days.";
            _pendingNotifications.Add(new Notification
            {
                PatronEmail = patronEmail,
                Message = message
            });
        }
        
        public void AddRecommendationNotification(string patronEmail, string bookTitle, string reason)
        {
            var message = $"RECOMMENDATION: Based on your reading history, you might enjoy '{bookTitle}'. Reason: {reason}";
            _pendingNotifications.Add(new Notification
            {
                PatronEmail = patronEmail,
                Message = message
            });
        }

        public List<Notification> GetPendingNotifications()
        {
            return _pendingNotifications;
        }

        public void MarkAsSent(Guid notificationId)
        {
            var notification = _pendingNotifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsSent = true;
            }
        }

        // Simulate sending notifications
        public void SendNotifications()
        {
            foreach (var notification in _pendingNotifications.Where(n => !n.IsSent))
            {
                Console.WriteLine($"Sending email to {notification.PatronEmail}: {notification.Message}");
                notification.IsSent = true;
            }
        }
    }
}