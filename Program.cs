using System;
using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem
{
    class Program
    {
        private static IBookService _bookService;

        private static void EnsureDataDirectoryExists()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataDirectory = Path.Combine(baseDirectory, "Data");
            
            if (!Directory.Exists(dataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(dataDirectory);
                    Console.WriteLine($"Created data directory at: {dataDirectory}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating data directory: {ex.Message}");
                    Console.WriteLine("The application may not be able to save data.");
                }
            }
        }

        static void Main(string[] args)
        {
            EnsureDataDirectoryExists();
            
            // Initialize repositories and services
            var bookRepository = new JsonRepository<Book>("books.json");
            var loanRepository = new JsonRepository<LoanRecord>("loans.json");
            _bookService = new BookService(bookRepository, loanRepository);

            // Add some sample data if the book repository is empty
            if (bookRepository.GetAll().Count == 0)
            {
                AddSampleData();
            }

            bool exit = false;
            while (!exit)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllBooks();
                        break;
                    case "2":
                        AddBook();
                        break;
                    case "3":
                        UpdateBook();
                        break;
                    case "4":
                        DeleteBook();
                        break;
                    case "5":
                        SearchBooks();
                        break;
                    case "6":
                        LendBook();
                        break;
                    case "7":
                        ReturnBook();
                        break;
                    case "8":
                        GetRecommendations();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void DisplayMenu()
        {
            Console.WriteLine("===== LIBRARY MANAGEMENT SYSTEM =====");
            Console.WriteLine("1. View All Books");
            Console.WriteLine("2. Add New Book");
            Console.WriteLine("3. Update Book");
            Console.WriteLine("4. Delete Book");
            Console.WriteLine("5. Search Books");
            Console.WriteLine("6. Lend Book");
            Console.WriteLine("7. Return Book");
            Console.WriteLine("8. Get Book Recommendations");
            Console.WriteLine("0. Exit");
            Console.Write("\nEnter your choice: ");
        }

        private static void ViewAllBooks()
        {
            var books = _bookService.GetAllBooks();
            
            Console.WriteLine("\n===== ALL BOOKS =====");
            
            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library.");
                return;
            }

            DisplayBooks(books);
        }

        private static void DisplayBooks(List<Book> books)
        {
            Console.WriteLine("\n{0,-36} {1,-30} {2,-20} {3,-15} {4,-10}", 
                "ID", "Title", "Author", "ISBN", "Available/Total");
            Console.WriteLine(new string('-', 115));
            
            foreach (var book in books)
            {
                Console.WriteLine("{0,-36} {1,-30} {2,-20} {3,-15} {4}/{5}", 
                    book.Id, 
                    book.Title.Length > 28 ? book.Title.Substring(0, 25) + "..." : book.Title,
                    book.Author.Length > 18 ? book.Author.Substring(0, 15) + "..." : book.Author,
                    book.ISBN,
                    book.AvailableQuantity,
                    book.Quantity);
            }
        }

        private static void AddBook()
        {
            Console.WriteLine("\n===== ADD NEW BOOK =====");
            
            try
            {
                var book = new Book();
                
                Console.Write("Title: ");
                book.Title = Console.ReadLine();
                
                Console.Write("Author: ");
                book.Author = Console.ReadLine();
                
                Console.Write("ISBN: ");
                book.ISBN = Console.ReadLine();
                
                Console.Write("Genre: ");
                book.Genre = Console.ReadLine();
                
                Console.Write("Publication Year: ");
                if (int.TryParse(Console.ReadLine(), out int year))
                    book.PublicationYear = year;
                
                Console.Write("Quantity: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                    book.Quantity = quantity;
                else
                    throw new ArgumentException("Quantity must be a positive number");
                
                _bookService.AddBook(book);
                Console.WriteLine("Book added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void UpdateBook()
        {
            Console.WriteLine("\n===== UPDATE BOOK =====");
            
            try
            {
                Console.Write("Enter Book ID: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    var book = _bookService.GetBookById(id);
                    
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }

                    Console.WriteLine($"\nCurrent Details:\nTitle: {book.Title}\nAuthor: {book.Author}\nISBN: {book.ISBN}\nGenre: {book.Genre}\nPublication Year: {book.PublicationYear}\nQuantity: {book.Quantity}");
                    Console.WriteLine("\nEnter new details (leave blank to keep current value):");
                    
                    Console.Write("Title: ");
                    string input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Title = input;
                    
                    Console.Write("Author: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Author = input;
                    
                    Console.Write("ISBN: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.ISBN = input;
                    
                    Console.Write("Genre: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        book.Genre = input;
                    
                    Console.Write("Publication Year: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int year))
                        book.PublicationYear = year;
                    
                    Console.Write("Quantity: ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int quantity) && quantity > 0)
                        book.Quantity = quantity;
                    
                    _bookService.UpdateBook(book);
                    Console.WriteLine("Book updated successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DeleteBook()
        {
            Console.WriteLine("\n===== DELETE BOOK =====");
            
            try
            {
                Console.Write("Enter Book ID: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    var book = _bookService.GetBookById(id);
                    
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }

                    Console.WriteLine($"\nBook Details:\nTitle: {book.Title}\nAuthor: {book.Author}\nISBN: {book.ISBN}");
                    Console.Write("\nAre you sure you want to delete this book? (y/n): ");
                    
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        _bookService.DeleteBook(id);
                        Console.WriteLine("Book deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void SearchBooks()
        {
            Console.WriteLine("\n===== SEARCH BOOKS =====");
            Console.WriteLine("Filter by:");
            Console.WriteLine("1. Title");
            Console.WriteLine("2. Author");
            Console.WriteLine("3. ISBN");
            Console.WriteLine("4. Genre");
            Console.WriteLine("5. Available Books Only");
            Console.WriteLine("6. All Fields");
            Console.Write("\nEnter your choice: ");
            
            string filterType = Console.ReadLine() switch
            {
                "1" => "title",
                "2" => "author",
                "3" => "isbn",
                "4" => "genre",
                "5" => "available",
                _ => "all"
            };
            
            if (filterType == "available")
            {
                var books = _bookService.SearchBooks("", filterType);
                DisplayBooks(books);
                return;
            }
            
            Console.Write("Enter search term: ");
            string searchTerm = Console.ReadLine();
            
            var searchResults = _bookService.SearchBooks(searchTerm, filterType);
            
            if (searchResults.Count == 0)
            {
                Console.WriteLine("No books found matching your search criteria.");
                return;
            }
            
            DisplayBooks(searchResults);
        }

        private static void LendBook()
        {
            Console.WriteLine("\n===== LEND BOOK =====");
            
            try
            {
                Console.Write("Enter Book ID: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    var book = _bookService.GetBookById(id);
                    
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }

                    if (book.AvailableQuantity <= 0)
                    {
                        Console.WriteLine("No copies available for lending.");
                        return;
                    }

                    Console.WriteLine($"\nBook Details:\nTitle: {book.Title}\nAuthor: {book.Author}\nAvailable: {book.AvailableQuantity}/{book.Quantity}");
                    
                    Console.Write("\nEnter borrower's name: ");
                    string borrowerName = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(borrowerName))
                    {
                        Console.WriteLine("Borrower name cannot be empty.");
                        return;
                    }

                    bool success = _bookService.LendBook(id, borrowerName);
                    
                    if (success)
                        Console.WriteLine("Book lent successfully!");
                    else
                        Console.WriteLine("Failed to lend book.");
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ReturnBook()
        {
            Console.WriteLine("\n===== RETURN BOOK =====");
            
            try
            {
                Console.Write("Enter Book ID: ");
                if (Guid.TryParse(Console.ReadLine(), out Guid id))
                {
                    var book = _bookService.GetBookById(id);
                    
                    if (book == null)
                    {
                        Console.WriteLine("Book not found.");
                        return;
                    }

                    if (book.AvailableQuantity >= book.Quantity)
                    {
                        Console.WriteLine("All copies have already been returned.");
                        return;
                    }

                    Console.WriteLine($"\nBook Details:\nTitle: {book.Title}\nAuthor: {book.Author}\nAvailable: {book.AvailableQuantity}/{book.Quantity}");
                    
                    Console.Write("\nEnter borrower's name: ");
                    string borrowerName = Console.ReadLine();
                    
                    if (string.IsNullOrWhiteSpace(borrowerName))
                    {
                        Console.WriteLine("Borrower name cannot be empty.");
                        return;
                    }

                    bool success = _bookService.ReturnBook(id, borrowerName);
                    
                    if (success)
                        Console.WriteLine("Book returned successfully!");
                    else
                        Console.WriteLine("Failed to return book. Check if this borrower has borrowed this book.");
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void GetRecommendations()
        {
            Console.WriteLine("\n===== BOOK RECOMMENDATIONS =====");
            
            Console.Write("Enter borrower's name: ");
            string borrowerName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(borrowerName))
            {
                Console.WriteLine("Borrower name cannot be empty.");
                return;
            }

            var recommendations = _bookService.GetRecommendedBooks(borrowerName);
            
            if (recommendations.Count == 0)
            {
                Console.WriteLine("No recommendations available for this borrower.");
                return;
            }

            Console.WriteLine($"\nRecommended books for {borrowerName}:");
            DisplayBooks(recommendations);
        }

        private static void AddSampleData()
        {
            var books = new List<Book>
            {
                new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "9780743273565",
                    Genre = "Classic",
                    PublicationYear = 1925,
                    Quantity = 5,
                    AvailableQuantity = 5
                },
                new Book
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    ISBN = "9780061120084",
                    Genre = "Fiction",
                    PublicationYear = 1960,
                    Quantity = 3,
                    AvailableQuantity = 3
                },
                new Book
                {
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "9780451524935",
                    Genre = "Dystopian",
                    PublicationYear = 1949,
                    Quantity = 4,
                    AvailableQuantity = 4
                }
            };

            foreach (var book in books)
            {
                _bookService.AddBook(book);
            }
        }
    }
}