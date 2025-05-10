using System;

namespace LibraryManagementSystem.Models
{
    public class LoanRecord
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BorrowerName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;

        public LoanRecord()
        {
            Id = Guid.NewGuid();
            BorrowDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14); // default 14 days loan period
        }
    }
}