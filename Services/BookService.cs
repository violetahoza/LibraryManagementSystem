using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class BookService
    {
        private readonly BookRepository _bookRepository;

        public BookService(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            book.AvailableQuantity = book.TotalQuantity;
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<bool> IsBookAvailableAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book != null && book.AvailableQuantity > 0;
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm, string searchBy = "all")
        {
            searchTerm = searchTerm.ToLower();
            
            return await _bookRepository.SearchAsync(book => 
            {
                switch (searchBy.ToLower())
                {
                    case "title":
                        return book.Title.ToLower().Contains(searchTerm);
                    case "author":
                        return book.Author.ToLower().Contains(searchTerm);
                    case "isbn":
                        return book.ISBN.ToLower().Contains(searchTerm);
                    case "genre":
                        return book.Genre.ToLower().Contains(searchTerm);
                    case "tags":
                        return book.Tags.Any(t => t.ToLower().Contains(searchTerm));
                    case "all":
                    default:
                        return book.Title.ToLower().Contains(searchTerm) ||
                               book.Author.ToLower().Contains(searchTerm) ||
                               book.ISBN.ToLower().Contains(searchTerm) ||
                               book.Genre.ToLower().Contains(searchTerm) ||
                               book.Tags.Any(t => t.ToLower().Contains(searchTerm));
                }
            });
        }

        // New feature: Get book recommendations based on popularity
        public async Task<List<Book>> GetPopularBooksAsync(int count = 5)
        {
            var allBooks = await _bookRepository.GetAllAsync();
            return allBooks.OrderByDescending(b => b.BorrowCount).Take(count).ToList();
        }

        // New feature: Get related books
        public async Task<List<Book>> GetRelatedBooksAsync(Guid bookId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                return new List<Book>();

            var allBooks = await _bookRepository.GetAllAsync();
            
            // Find books with the same author or genre or matching tags
            return allBooks
                .Where(b => b.Id != bookId && 
                          (b.Author == book.Author || 
                           b.Genre == book.Genre || 
                           b.Tags.Intersect(book.Tags).Any()))
                .OrderByDescending(b => 
                    (b.Author == book.Author ? 3 : 0) + 
                    (b.Genre == book.Genre ? 2 : 0) + 
                    b.Tags.Intersect(book.Tags).Count())
                .Take(5)
                .ToList();
        }
    }
}