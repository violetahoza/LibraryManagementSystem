using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class BookRepository : IRepository<Book>
    {
        private readonly string _filePath;
        private List<Book> _books;

        public BookRepository(string filePath)
        {
            _filePath = filePath;
            _books = LoadBooksFromFile().GetAwaiter().GetResult();
        }

        private async Task<List<Book>> LoadBooksFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Book>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            catch
            {
                return new List<Book>();
            }
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return _books;
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public async Task AddAsync(Book book)
        {
            _books.Add(book);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook != null)
            {
                var index = _books.IndexOf(existingBook);
                _books[index] = book;
                await SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _books.Remove(book);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            var json = JsonSerializer.Serialize(_books, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Custom methods for searching
        public async Task<List<Book>> SearchAsync(Func<Book, bool> predicate)
        {
            return _books.Where(predicate).ToList();
        }
    }
}