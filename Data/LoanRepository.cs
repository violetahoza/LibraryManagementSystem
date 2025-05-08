using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class LoanRepository : IRepository<Loan>
    {
        private readonly string _filePath;
        private List<Loan> _loans;

        public LoanRepository(string filePath)
        {
            _filePath = filePath;
            _loans = LoadLoansFromFile().GetAwaiter().GetResult();
        }

        private async Task<List<Loan>> LoadLoansFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Loan>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<List<Loan>>(json) ?? new List<Loan>();
            }
            catch
            {
                return new List<Loan>();
            }
        }

        public async Task<List<Loan>> GetAllAsync()
        {
            return _loans;
        }

        public async Task<Loan> GetByIdAsync(Guid id)
        {
            return _loans.FirstOrDefault(l => l.Id == id);
        }

        public async Task AddAsync(Loan loan)
        {
            _loans.Add(loan);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Loan loan)
        {
            var existingLoan = _loans.FirstOrDefault(l => l.Id == loan.Id);
            if (existingLoan != null)
            {
                var index = _loans.IndexOf(existingLoan);
                _loans[index] = loan;
                await SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var loan = _loans.FirstOrDefault(l => l.Id == id);
            if (loan != null)
            {
                _loans.Remove(loan);
                await SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync()
        {
            var json = JsonSerializer.Serialize(_loans, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Custom methods
        public async Task<List<Loan>> GetActiveLoansAsync()
        {
            return _loans.Where(l => !l.IsReturned).ToList();
        }

        public async Task<List<Loan>> GetLoansByBookIdAsync(Guid bookId)
        {
            return _loans.Where(l => l.BookId == bookId).ToList();
        }

        public async Task<List<Loan>> GetOverdueLoansAsync()
        {
            return _loans.Where(l => !l.IsReturned && l.DueDate < DateTime.Now).ToList();
        }
    }
}