using System;
using System.Collections.Generic;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        Book GetBookById(Guid id);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(Guid id);
        List<Book> SearchBooks(string searchTerm, string filterType);
        bool LendBook(Guid bookId, string borrowerName);
        bool ReturnBook(Guid bookId, string borrowerName);
        List<Book> GetRecommendedBooks(string borrowerName); // the additional feature
    }
}
