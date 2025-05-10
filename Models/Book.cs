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
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Genre { get; set; }
        public int PublicationYear { get; set; }
        public DateTime DateAdded { get; set; }

        public Book()
        {
            Id = Guid.NewGuid();
            DateAdded = DateTime.Now;
        }
    }
}
