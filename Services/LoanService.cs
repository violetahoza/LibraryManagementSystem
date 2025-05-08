using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public class LoanService
    {
        private readonly LoanRepository _loanRepository;
        private readonly BookRepository _bookRepository;

        public LoanService(LoanRepository loanRepository, BookRepository bookRepository)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
        }

        public async Task<List<Loan>> GetAllLoansAsync()
        {
            return await _loanRepository.GetAllAsync();
        }

        public async Task<List<Loan>> GetActiveLoansAsync()
        {
            return await _loanRepository.GetActiveLoansAsync();
        }

        public async Task<List<Loan>> GetOverdueLoansAsync()
        {
            return await _loanRepository.GetOverdueLoansAsync();
        }

        public async Task<Loan> GetLoanByIdAsync(Guid id)
        {
            return await _loanRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> BorrowBookAsync(Guid bookId, string patronName, string patronEmail, int loanDays = 14)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);
            
            if (book == null)
                return (false, "Book not found.");
                
            if (book.AvailableQuantity <= 0)
                return (false, "No copies of this book are available for borrowing.");

            // Create the loan
            var loan = new Loan
            {
                BookId = bookId,
                PatronName = patronName,
                PatronEmail = patronEmail,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays)
            };

            // Update book
            book.AvailableQuantity--;
            book.LastBorrowedDate = DateTime.Now;
            book.BorrowCount++;

            await _loanRepository.AddAsync(loan);
            await _bookRepository.UpdateAsync(book);

            return (true, $"Book '{book.Title}' has been borrowed successfully. Due date: {loan.DueDate.ToShortDateString()}");
        }

        public async Task<(bool Success, string Message)> ReturnBookAsync(Guid loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            
            if (loan == null)
                return (false, "Loan record not found.");
                
            if (loan.IsReturned)
                return (false, "This book has already been returned.");

            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            
            if (book == null)
                return (false, "Book record not found.");
                
            if (book.AvailableQuantity >= book.TotalQuantity)
                return (false, "Cannot return book as all copies are already available. This is likely a data inconsistency.");

            // Update loan
            loan.ReturnDate = DateTime.Now;
            
            // Update book
            book.AvailableQuantity++;

            await _loanRepository.UpdateAsync(loan);
            await _bookRepository.UpdateAsync(book);

            return (true, $"Book '{book.Title}' has been returned successfully.");
        }

        // Helper to get book details for a loan
        public async Task<Book> GetBookForLoanAsync(Guid loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
                return null;
                
            return await _bookRepository.GetByIdAsync(loan.BookId);
        }
    }
}