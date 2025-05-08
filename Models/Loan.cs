using System;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string PatronName { get; set; }
        public string PatronEmail { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;

        public Loan()
        {
            Id = Guid.NewGuid();
        }
    }
}