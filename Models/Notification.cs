using System;

namespace LibraryManagementSystem.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string PatronEmail { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsSent { get; set; }

        public Notification()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            IsSent = false;
        }
    }
}