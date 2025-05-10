using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<LoanRecord> _loanRepository;

        public BookService(IRepository<Book> bookRepository, IRepository<LoanRecord> loanRepository)
        {
            _bookRepository = bookRepository;
            _loanRepository = loanRepository;
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public Book GetBookById(Guid id)
        {
            return _bookRepository.GetById(id);
        }

        public void AddBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty");

            if (book.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            book.AvailableQuantity = book.Quantity;
            _bookRepository.Add(book);
            _bookRepository.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            var existingBook = _bookRepository.GetById(book.Id);
            if (existingBook == null)
                throw new ArgumentException($"Book with ID {book.Id} not found");

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title cannot be empty");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author cannot be empty");

            if (book.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative");

            // Ensure available quantity isn't greater than total quantity
            int loaned = existingBook.Quantity - existingBook.AvailableQuantity;
            book.AvailableQuantity = Math.Max(0, book.Quantity - loaned);

            _bookRepository.Update(book);
            _bookRepository.SaveChanges();
        }

        public void DeleteBook(Guid id)
        {
            var book = _bookRepository.GetById(id);
            if (book == null)
                throw new ArgumentException($"Book with ID {id} not found");

            // Check if all copies are available (none are borrowed)
            if (book.AvailableQuantity < book.Quantity)
                throw new InvalidOperationException("Cannot delete book with borrowed copies");

            _bookRepository.Delete(id);
            _bookRepository.SaveChanges();
        }

        public List<Book> SearchBooks(string searchTerm, string filterType)
        {
            var books = _bookRepository.GetAll();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return books;

            searchTerm = searchTerm.ToLower();

            return filterType.ToLower() switch
            {
                "title" => books.Where(b => b.Title.ToLower().Contains(searchTerm)).ToList(),
                "author" => books.Where(b => b.Author.ToLower().Contains(searchTerm)).ToList(),
                "isbn" => books.Where(b => b.ISBN.ToLower().Contains(searchTerm)).ToList(),
                "genre" => books.Where(b => b.Genre?.ToLower().Contains(searchTerm) ?? false).ToList(),
                "available" => books.Where(b => b.AvailableQuantity > 0).ToList(),
                _ => books.Where(b => 
                    b.Title.ToLower().Contains(searchTerm) || 
                    b.Author.ToLower().Contains(searchTerm) || 
                    b.ISBN.ToLower().Contains(searchTerm) ||
                    (b.Genre?.ToLower().Contains(searchTerm) ?? false)).ToList()
            };
        }

        public bool LendBook(Guid bookId, string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                throw new ArgumentException("Borrower name cannot be empty");

            var book = _bookRepository.GetById(bookId);
            if (book == null)
                throw new ArgumentException($"Book with ID {bookId} not found");

            if (book.AvailableQuantity <= 0)
                return false;

            // Create loan record
            var loan = new LoanRecord
            {
                BookId = bookId,
                BorrowerName = borrowerName
            };

            // Update book availability
            book.AvailableQuantity--;
            
            _loanRepository.Add(loan);
            _bookRepository.Update(book);
            
            _loanRepository.SaveChanges();
            _bookRepository.SaveChanges();
            
            return true;
        }

        public bool ReturnBook(Guid bookId, string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                throw new ArgumentException("Borrower name cannot be empty");

            var book = _bookRepository.GetById(bookId);
            if (book == null)
                throw new ArgumentException($"Book with ID {bookId} not found");

            if (book.AvailableQuantity >= book.Quantity)
                return false; // All copies are already returned

            // Find the loan record
            var loans = _loanRepository.GetAll();
            var loan = loans.FirstOrDefault(l => 
                l.BookId == bookId && 
                l.BorrowerName.Equals(borrowerName, StringComparison.OrdinalIgnoreCase) && 
                !l.IsReturned);

            if (loan == null)
                return false;

            // Update loan and book
            loan.ReturnDate = DateTime.Now;
            book.AvailableQuantity++;
            
            _loanRepository.Update(loan);
            _bookRepository.Update(book);
            
            _loanRepository.SaveChanges();
            _bookRepository.SaveChanges();
            
            return true;
        }

        // Additional feature - Book recommendation system based on borrower's history
        public List<Book> GetRecommendedBooks(string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return new List<Book>();

            var loans = _loanRepository.GetAll()
                .Where(l => l.BorrowerName.Equals(borrowerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!loans.Any())
                return new List<Book>();

            // Find genres and authors the borrower has previously borrowed
            var borrowedBookIds = loans.Select(l => l.BookId).Distinct().ToList();
            var borrowedBooks = borrowedBookIds
                .Select(id => _bookRepository.GetById(id))
                .Where(b => b != null)
                .ToList();

            var genres = borrowedBooks
                .Where(b => !string.IsNullOrEmpty(b.Genre))
                .Select(b => b.Genre)
                .Distinct()
                .ToList();

            var authors = borrowedBooks
                .Select(b => b.Author)
                .Distinct()
                .ToList();

            // Find books with similar genres or by the same authors
            var allBooks = _bookRepository.GetAll();
            var recommendedBooks = allBooks
                .Where(b => !borrowedBookIds.Contains(b.Id) && 
                           (genres.Contains(b.Genre) || authors.Contains(b.Author)) &&
                           b.AvailableQuantity > 0)
                .Take(5)
                .ToList();

            return recommendedBooks;
        }
    }
}