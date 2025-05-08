using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? LastBorrowedDate { get; set; }
        public int BorrowCount { get; set; } // track popularity
        public List<string> Tags { get; set; } = new List<string>();

        public Book()
        {
            Id = Guid.NewGuid();
            AddedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} ({PublicationYear}) - {AvailableQuantity}/{TotalQuantity} available";
        }
    }
}