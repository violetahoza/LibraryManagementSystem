using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.UI
{
    public class ConsoleUI
    {
        private readonly BookService _bookService;
        private readonly LoanService _loanService;
        private readonly NotificationService _notificationService;

        public ConsoleUI(BookService bookService, LoanService loanService, NotificationService notificationService)
        {
            _bookService = bookService;
            _loanService = loanService;
            _notificationService = notificationService;
        }

        public async Task RunAsync()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("==== LIBRARY MANAGEMENT SYSTEM ====");
                Console.WriteLine("1. Manage Books");
                Console.WriteLine("2. Manage Loans");
                Console.WriteLine("3. Search Books");
                Console.WriteLine("4. Book Recommendations");
                Console.WriteLine("5. Notifications");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await ManageBooksMenuAsync();
                            break;
                        case 2:
                            await ManageLoansMenuAsync();
                            break;
                        case 3:
                            await SearchBooksAsync();
                            break;
                        case 4:
                            await BookRecommendationsMenuAsync();
                            break;
                        case 5:
                            await ManageNotificationsAsync();
                            break;
                        case 0:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private async Task ManageBooksMenuAsync()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("==== MANAGE BOOKS ====");
                Console.WriteLine("1. View All Books");
                Console.WriteLine("2. Add New Book");
                Console.WriteLine("3. Edit Book");
                Console.WriteLine("4. Delete Book");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await ViewAllBooksAsync();
                            break;
                        case 2:
                            await AddBookAsync();
                            break;
                        case 3:
                            await EditBookAsync();
                            break;
                        case 4:
                            await DeleteBookAsync();
                            break;
                        case 0:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private async Task ViewAllBooksAsync()
        {
            Console.Clear();
            Console.WriteLine("==== ALL BOOKS ====\n");

            var books = await _bookService.GetAllBooksAsync();

            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library.");
            }
            else
            {
                foreach (var book in books)
                {
                    Console.WriteLine($"ID: {book.Id}");
                    Console.WriteLine($"Title: {book.Title}");
                    Console.WriteLine($"Author: {book.Author}");
                    Console.WriteLine($"ISBN: {book.ISBN}");
                    Console.WriteLine($"Year: {book.PublicationYear}");
                    Console.WriteLine($"Publisher: {book.Publisher}");
                    Console.WriteLine($"Genre: {book.Genre}");
                    Console.WriteLine($"Quantity: {book.AvailableQuantity}/{book.TotalQuantity}");
                    Console.WriteLine($"Tags: {string.Join(", ", book.Tags)}");
                    Console.WriteLine($"Borrow Count: {book.BorrowCount}");
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task AddBookAsync()
        {
            Console.Clear();
            Console.WriteLine("==== ADD NEW BOOK ====\n");

            var book = new Book();

            Console.Write("Title: ");
            book.Title = Console.ReadLine();

            Console.Write("Author: ");
            book.Author = Console.ReadLine();

            Console.Write("ISBN: ");
            book.ISBN = Console.ReadLine();

            Console.Write("Publication Year: ");
            if (int.TryParse(Console.ReadLine(), out int year))
                book.PublicationYear = year;

            Console.Write("Publisher: ");
            book.Publisher = Console.ReadLine();

            Console.Write("Genre: ");
            book.Genre = Console.ReadLine();

            Console.Write("Total Quantity: ");
            if (int.TryParse(Console.ReadLine(), out int quantity))
                book.TotalQuantity = quantity;

            Console.Write("Tags (comma separated): ");
            var tagsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(tagsInput))
            {
                book.Tags = tagsInput.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();
            }

            await _bookService.AddBookAsync(book);

            Console.WriteLine("\nBook added successfully. Press any key to continue...");
            Console.ReadKey();
        }

        private async Task EditBookAsync()
        {
            Console.Clear();
            Console.WriteLine("==== EDIT BOOK ====\n");

            Console.Write("Enter Book ID: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid id))
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book != null)
                {
                    Console.WriteLine($"Editing Book: {book.Title} by {book.Author}");

                    Console.Write($"Title [{book.Title}]: ");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Title = input;

                    Console.Write($"Author [{book.Author}]: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Author = input;

                    Console.Write($"ISBN [{book.ISBN}]: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.ISBN = input;

                    Console.Write($"Publication Year [{book.PublicationYear}]: ");
                    input = Console.ReadLine();
                    if (int.TryParse(input, out int year))
                        book.PublicationYear = year;

                    Console.Write($"Publisher [{book.Publisher}]: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Publisher = input;

                    Console.Write($"Genre [{book.Genre}]: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Genre = input;

                    Console.Write($"Total Quantity [{book.TotalQuantity}]: ");
                    input = Console.ReadLine();
                    if (int.TryParse(input, out int quantity))
                    {
                        // Ensure total quantity is not less than copies already on loan
                        int copiesOnLoan = book.TotalQuantity - book.AvailableQuantity;
                        if (quantity < copiesOnLoan)
                        {
                            Console.WriteLine($"Cannot set total quantity less than copies on loan ({copiesOnLoan}).");
                        }
                        else
                        {
                            // Update available quantity accordingly
                            book.AvailableQuantity += (quantity - book.TotalQuantity);
                            book.TotalQuantity = quantity;
                        }
                    }

                    Console.Write($"Tags [{string.Join(", ", book.Tags)}]: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        book.Tags = input.Split(',')
                            .Select(t => t.Trim())
                            .Where(t => !string.IsNullOrWhiteSpace(t))
                            .ToList();
                    }

                    await _bookService.UpdateBookAsync(book);
                    Console.WriteLine("\nBook updated successfully.");
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task DeleteBookAsync()
        {
            Console.Clear();
            Console.WriteLine("==== DELETE BOOK ====\n");

            Console.Write("Enter Book ID: ");
            if (Guid.TryParse(Console.ReadLine(), out Guid id))
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book != null)
                {
                    Console.WriteLine($"Are you sure you want to delete '{book.Title}' by {book.Author}? (y/n)");
                    var confirmation = Console.ReadLine().ToLower();
                    
                    if (confirmation == "y" || confirmation == "yes")
                    {
                        // Check if any copies are on loan
                        if (book.AvailableQuantity < book.TotalQuantity)
                        {
                            Console.WriteLine("This book has copies currently on loan. Cannot delete.");
                        }
                        else
                        {
                            await _bookService.DeleteBookAsync(id);
                            Console.WriteLine("Book deleted successfully.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                else
                {
                    Console.WriteLine("Book not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ManageLoansMenuAsync()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("==== MANAGE LOANS ====");
                Console.WriteLine("1. View All Loans");
                Console.WriteLine("2. View Active Loans");
                Console.WriteLine("3. View Overdue Loans");
                Console.WriteLine("4. Borrow Book");
                Console.WriteLine("5. Return Book");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await ViewAllLoansAsync();
                            break;
                        case 2:
                            await ViewActiveLoansAsync();
                            break;
                        case 3:
                            await ViewOverdueLoansAsync();
                            break;
                        case 4:
                            await BorrowBookAsync();
                            break;
                        case 5:
                            await ReturnBookAsync();
                            break;
                        case 0:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private async Task ViewAllLoansAsync()
        {
            Console.Clear();
            Console.WriteLine("==== ALL LOANS ====\n");

            var loans = await _loanService.GetAllLoansAsync();

            if (loans.Count == 0)
            {
                Console.WriteLine("No loan records found.");
            }
            else
            {
                foreach (var loan in loans)
                {
                    var book = await _loanService.GetBookForLoanAsync(loan.Id);
                    
                    Console.WriteLine($"Loan ID: {loan.Id}");
                    Console.WriteLine($"Book: {book?.Title ?? "Unknown"}");
                    Console.WriteLine($"Patron: {loan.PatronName} ({loan.PatronEmail})");
                    Console.WriteLine($"Borrowed: {loan.BorrowDate.ToShortDateString()}");
                    Console.WriteLine($"Due: {loan.DueDate.ToShortDateString()}");
                    Console.WriteLine($"Status: {(loan.IsReturned ? $"Returned on {loan.ReturnDate?.ToShortDateString()}" : "On Loan")}");
                    
                    if (!loan.IsReturned && loan.DueDate < DateTime.Now)
                    {
                        Console.WriteLine("STATUS: OVERDUE");
                    }
                    
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ViewActiveLoansAsync()
        {
            Console.Clear();
            Console.WriteLine("==== ACTIVE LOANS ====\n");

            var loans = await _loanService.GetActiveLoansAsync();

            if (loans.Count == 0)
            {
                Console.WriteLine("No active loans found.");
            }
            else
            {
                foreach (var loan in loans)
                {
                    var book = await _loanService.GetBookForLoanAsync(loan.Id);
                    
                    Console.WriteLine($"Loan ID: {loan.Id}");
                    Console.WriteLine($"Book: {book?.Title ?? "Unknown"}");
                    Console.WriteLine($"Patron: {loan.PatronName} ({loan.PatronEmail})");
                    Console.WriteLine($"Borrowed: {loan.BorrowDate.ToShortDateString()}");
                    Console.WriteLine($"Due: {loan.DueDate.ToShortDateString()}");
                    
                    if (loan.DueDate < DateTime.Now)
                    {
                        Console.WriteLine("STATUS: OVERDUE");
                    }
                    
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ViewOverdueLoansAsync()
        {
            Console.Clear();
            Console.WriteLine("==== OVERDUE LOANS ====\n");

            var loans = await _loanService.GetOverdueLoansAsync();

            if (loans.Count == 0)
            {
                Console.WriteLine("No overdue loans found.");
            }
            else
            {
                foreach (var loan in loans)
                {
                    var book = await _loanService.GetBookForLoanAsync(loan.Id);
                    
                    Console.WriteLine($"Loan ID: {loan.Id}");
                    Console.WriteLine($"Book: {book?.Title ?? "Unknown"}");
                    Console.WriteLine($"Patron: {loan.PatronName} ({loan.PatronEmail})");
                    Console.WriteLine($"Borrowed: {loan.BorrowDate.ToShortDateString()}");
                    Console.WriteLine($"Due: {loan.DueDate.ToShortDateString()}");
                    Console.WriteLine($"Days Overdue: {(DateTime.Now - loan.DueDate).Days}");
                    
                    // Create notification for overdue book
                    _notificationService.AddOverdueNotification(loan.PatronEmail, book?.Title ?? "Unknown", loan.DueDate);
                    
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task BorrowBookAsync()
        {
            Console.Clear();
            Console.WriteLine("==== BORROW BOOK ====\n");

            // First, search for the book
            Console.Write("Search for book (title/author/ISBN): ");
            var searchTerm = Console.ReadLine();
            
            var books = await _bookService.SearchBooksAsync(searchTerm);
            
            if (books.Count == 0)
            {
                Console.WriteLine("No books found matching your search.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display the search results
            Console.WriteLine("\nSearch Results:");
            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {books[i].Title} by {books[i].Author} - Available: {books[i].AvailableQuantity}/{books[i].TotalQuantity}");
            }

            // Let the user select a book
            Console.Write("\nSelect a book (number) or 0 to cancel: ");
            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= books.Count)
            {
                var selectedBook = books[selection - 1];
                
                if (selectedBook.AvailableQuantity <= 0)
                {
                    Console.WriteLine("This book is not available for borrowing.");
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    return;
                }

                // Get patron details
                Console.Write("Patron Name: ");
                var patronName = Console.ReadLine();
                
                Console.Write("Patron Email: ");
                var patronEmail = Console.ReadLine();
                
                Console.Write("Loan Duration (days, default 14): ");
                var durationInput = Console.ReadLine();
                int duration = 14;
                if (!string.IsNullOrWhiteSpace(durationInput) && int.TryParse(durationInput, out int customDuration))
                {
                    duration = customDuration;
                }

                // Process the loan
                var result = await _loanService.BorrowBookAsync(selectedBook.Id, patronName, patronEmail, duration);
                
                Console.WriteLine(result.Message);
                
                // Generate a recommendation based on the borrowed book
                var recommendations = await _bookService.GetRelatedBooksAsync(selectedBook.Id);
                if (recommendations.Count > 0)
                {
                    var recommendation = recommendations.FirstOrDefault(r => r.AvailableQuantity > 0);
                    if (recommendation != null)
                    {
                        _notificationService.AddRecommendationNotification(
                            patronEmail, 
                            recommendation.Title,
                            $"You might enjoy this {recommendation.Genre} book by {recommendation.Author}"
                        );
                    }
                }
            }
            else if (selection != 0)
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ReturnBookAsync()
        {
            Console.Clear();
            Console.WriteLine("==== RETURN BOOK ====\n");

            Console.WriteLine("1. Return by Loan ID");
            Console.WriteLine("2. Find Loan by Patron Email");
            Console.Write("\nEnter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter Loan ID: ");
                        if (Guid.TryParse(Console.ReadLine(), out Guid loanId))
                        {
                            var result = await _loanService.ReturnBookAsync(loanId);
                            Console.WriteLine(result.Message);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID format.");
                        }
                        break;
                        
                    case 2:
                        Console.Write("Enter Patron Email: ");
                        var email = Console.ReadLine();
                        
                        var allLoans = await _loanService.GetActiveLoansAsync();
                        var patronLoans = allLoans.Where(l => l.PatronEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();
                        
                        if (patronLoans.Count == 0)
                        {
                            Console.WriteLine("No active loans found for this patron.");
                        }
                        else
                        {
                            Console.WriteLine("\nActive Loans for this Patron:");
                            for (int i = 0; i < patronLoans.Count; i++)
                            {
                                var loan = patronLoans[i];
                                var book = await _loanService.GetBookForLoanAsync(loan.Id);
                                Console.WriteLine($"{i + 1}. {book?.Title ?? "Unknown"} - Due: {loan.DueDate.ToShortDateString()}");
                            }

                            Console.Write("\nSelect a loan to return (number) or 0 to cancel: ");
                            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= patronLoans.Count)
                            {
                                var selectedLoan = patronLoans[selection - 1];
                                var result = await _loanService.ReturnBookAsync(selectedLoan.Id);
                                Console.WriteLine(result.Message);
                            }
                            else if (selection != 0)
                            {
                                Console.WriteLine("Invalid selection.");
                            }
                        }
                        break;
                        
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task SearchBooksAsync()
        {
            Console.Clear();
            Console.WriteLine("==== SEARCH BOOKS ====\n");

            Console.WriteLine("Search by:");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Author");
            Console.WriteLine("3. ISBN");
            Console.WriteLine("4. Genre");
            Console.WriteLine("5. Tags");
            Console.WriteLine("6. All Fields");
            Console.Write("\nEnter your choice: ");

            string searchBy = "all";
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1: searchBy = "title"; break;
                    case 2: searchBy = "author"; break;
                    case 3: searchBy = "isbn"; break;
                    case 4: searchBy = "genre"; break;
                    case 5: searchBy = "tags"; break;
                    case 6: searchBy = "all"; break;
                    default: 
                        Console.WriteLine("Invalid choice. Searching all fields.");
                        break;
                }
            }

            Console.Write("\nEnter search term: ");
            var searchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Search term cannot be empty.");
            }
            else
            {
                var books = await _bookService.SearchBooksAsync(searchTerm, searchBy);

                Console.WriteLine($"\nFound {books.Count} books:\n");

                if (books.Count > 0)
                {
                    foreach (var book in books)
                    {
                        Console.WriteLine($"Title: {book.Title}");
                        Console.WriteLine($"Author: {book.Author}");
                        Console.WriteLine($"ISBN: {book.ISBN}");
                        Console.WriteLine($"Genre: {book.Genre}");
                        Console.WriteLine($"Available: {book.AvailableQuantity}/{book.TotalQuantity}");
                        Console.WriteLine(new string('-', 40));
                    }
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task BookRecommendationsMenuAsync()
        {
            bool back = false;

            while (!back)
            {
                Console.Clear();
                Console.WriteLine("==== BOOK RECOMMENDATIONS ====");
                Console.WriteLine("1. Most Popular Books");
                Console.WriteLine("2. Get Related Books");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nEnter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            await ViewPopularBooksAsync();
                            break;
                        case 2:
                            await ViewRelatedBooksAsync();
                            break;
                        case 0:
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private async Task ViewPopularBooksAsync()
        {
            Console.Clear();
            Console.WriteLine("==== MOST POPULAR BOOKS ====\n");

            Console.Write("Number of books to show (default 5): ");
            var input = Console.ReadLine();
            int count = 5;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int customCount))
            {
                count = customCount;
            }

            var popularBooks = await _bookService.GetPopularBooksAsync(count);

            if (popularBooks.Count == 0)
            {
                Console.WriteLine("No books found in the library.");
            }
            else
            {
                Console.WriteLine("Top books by borrow count:\n");
                
                for (int i = 0; i < popularBooks.Count; i++)
                {
                    var book = popularBooks[i];
                    Console.WriteLine($"{i + 1}. {book.Title} by {book.Author}");
                    Console.WriteLine($"   Genre: {book.Genre}");
                    Console.WriteLine($"   Borrow Count: {book.BorrowCount}");
                    Console.WriteLine($"   Available: {book.AvailableQuantity}/{book.TotalQuantity}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ViewRelatedBooksAsync()
        {
            Console.Clear();
            Console.WriteLine("==== RELATED BOOKS ====\n");

            // First, search for the reference book
            Console.Write("Search for a book (title/author): ");
            var searchTerm = Console.ReadLine();
            
            var books = await _bookService.SearchBooksAsync(searchTerm);
            
            if (books.Count == 0)
            {
                Console.WriteLine("No books found matching your search.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            // Display the search results
            Console.WriteLine("\nSearch Results:");
            for (int i = 0; i < books.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {books[i].Title} by {books[i].Author}");
            }

            // Let the user select a book
            Console.Write("\nSelect a book to find related titles (number) or 0 to cancel: ");
            if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= books.Count)
            {
                var selectedBook = books[selection - 1];
                var relatedBooks = await _bookService.GetRelatedBooksAsync(selectedBook.Id);
                
                Console.Clear();
                Console.WriteLine($"Books related to \"{selectedBook.Title}\" by {selectedBook.Author}:\n");
                
                if (relatedBooks.Count == 0)
                {
                    Console.WriteLine("No related books found.");
                }
                else
                {
                    foreach (var book in relatedBooks)
                    {
                        Console.WriteLine($"Title: {book.Title}");
                        Console.WriteLine($"Author: {book.Author}");
                        Console.WriteLine($"Genre: {book.Genre}");
                        Console.WriteLine($"Available: {book.AvailableQuantity}/{book.TotalQuantity}");
                        
                        // Show why this book is related
                        var relationReasons = new List<string>();
                        if (book.Author == selectedBook.Author)
                            relationReasons.Add("same author");
                        if (book.Genre == selectedBook.Genre)
                            relationReasons.Add("same genre");
                        var commonTags = book.Tags.Intersect(selectedBook.Tags).ToList();
                        if (commonTags.Any())
                            relationReasons.Add($"common tags: {string.Join(", ", commonTags)}");
                            
                        Console.WriteLine($"Related because: {string.Join(", ", relationReasons)}");
                        Console.WriteLine(new string('-', 40));
                    }
                }
            }
            else if (selection != 0)
            {
                Console.WriteLine("Invalid selection.");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ManageNotificationsAsync()
        {
            Console.Clear();
            Console.WriteLine("==== NOTIFICATIONS ====\n");

            var notifications = _notificationService.GetPendingNotifications();
            
            if (notifications.Count == 0)
            {
                Console.WriteLine("No pending notifications.");
            }
            else
            {
                Console.WriteLine($"Pending Notifications ({notifications.Count}):\n");
                
                foreach (var notification in notifications)
                {
                    Console.WriteLine($"To: {notification.PatronEmail}");
                    Console.WriteLine($"Message: {notification.Message}");
                    Console.WriteLine($"Created: {notification.CreatedDate.ToShortDateString()}");
                    Console.WriteLine($"Status: {(notification.IsSent ? "Sent" : "Pending")}");
                    Console.WriteLine(new string('-', 40));
                }
                
                Console.WriteLine("\n1. Send All Notifications");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("\nEnter your choice: ");
                
                if (int.TryParse(Console.ReadLine(), out int choice) && choice == 1)
                {
                    _notificationService.SendNotifications();
                    Console.WriteLine("\nAll notifications have been sent.");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}