using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.UI;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==== LIBRARY MANAGEMENT SYSTEM ====");
            Console.WriteLine("Choose interface:");
            Console.WriteLine("1. Console UI");
            Console.WriteLine("2. Web UI");
            Console.Write("\nEnter your choice (1 or 2): ");
            
            var choice = Console.ReadLine();
            
            // Initialize the repositories
            string dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            Directory.CreateDirectory(dataFolder);
            
            var bookRepository = new BookRepository(Path.Combine(dataFolder, "books.json"));
            var loanRepository = new LoanRepository(Path.Combine(dataFolder, "loans.json"));
            
            // Initialize the services
            var bookService = new BookService(bookRepository);
            var loanService = new LoanService(loanRepository, bookRepository);
            var notificationService = new NotificationService();
            
            if (choice == "1")
            {
                // Run Console UI
                var ui = new ConsoleUI(bookService, loanService, notificationService);
                await ui.RunAsync();
            }
            else
            {
                // Run Web UI
                await RunWebServerAsync(bookService, loanService, notificationService);
            }
        }

        static async Task RunWebServerAsync(BookService bookService, LoanService loanService, NotificationService notificationService)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "UI", "WebUI");
            
            // Check if the directory exists
            if (!Directory.Exists(webRootPath))
            {
                Console.WriteLine($"Error: Web UI folder not found at {webRootPath}");
                Console.WriteLine("Please create the UI/WebUI folder and add the HTML files.");
                return;
            }
            
            // Start the web server
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            
            Console.WriteLine("\nWeb server started successfully!");
            Console.WriteLine("Access the Library Management System at: http://localhost:8080/");
            Console.WriteLine("Press Ctrl+C to stop the server");
            
            // Handle requests
            try
            {
                while (true)
                {
                    var context = await listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context, webRootPath, bookService, loanService, notificationService));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        static async Task HandleRequestAsync(HttpListenerContext context, string webRootPath, 
            BookService bookService, LoanService loanService, NotificationService notificationService)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;
                
                // Get the requested path
                string path = request.Url.AbsolutePath;
                if (path == "/")
                    path = "/index.html";
                
                // Get the full file path
                string filePath = Path.Combine(webRootPath, path.TrimStart('/'));
                
                if (File.Exists(filePath))
                {
                    // Serve static file
                    response.ContentType = GetContentType(filePath);
                    byte[] buffer = File.ReadAllBytes(filePath);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else if (path.StartsWith("/api/"))
                {
                    // Handle API request - this is where you'll connect to your services
                    await HandleApiRequestAsync(path, request, response, bookService, loanService, notificationService);
                }
                else
                {
                    // 404 Not Found
                    response.StatusCode = 404;
                    byte[] buffer = Encoding.UTF8.GetBytes("404 - Page Not Found");
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                byte[] buffer = Encoding.UTF8.GetBytes($"Server Error: {ex.Message}");
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                context.Response.Close();
            }
        }

        static string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".html": return "text/html";
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                case ".json": return "application/json";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".gif": return "image/gif";
                default: return "application/octet-stream";
            }
        }

        static async Task HandleApiRequestAsync(string path, HttpListenerRequest request, 
            HttpListenerResponse response, BookService bookService, LoanService loanService, 
            NotificationService notificationService)
        {
            // Parse the API endpoint
            string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length < 2)
            {
                response.StatusCode = 400;
                return;
            }
            
            string controller = segments[1];
            string action = segments.Length > 2 ? segments[2] : "";
            string id = segments.Length > 3 ? segments[3] : "";
            
            // Set response content type to JSON
            response.ContentType = "application/json";
            
            try
            {
                string jsonResponse = "";
                
                if (controller == "books")
                {
                    if (string.IsNullOrEmpty(action))
                    {
                        if (request.HttpMethod == "GET")
                        {
                            // GET /api/books - Get all books
                            var books = await bookService.GetAllBooksAsync();
                            jsonResponse = JsonSerializer.Serialize(books);
                        }
                        else if (request.HttpMethod == "POST")
                        {
                            // POST /api/books - Add a new book
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string requestBody = await reader.ReadToEndAsync();
                                var book = JsonSerializer.Deserialize<Book>(requestBody);
                                
                                await bookService.AddBookAsync(book);
                                response.StatusCode = 201; // Created
                                jsonResponse = JsonSerializer.Serialize(book);
                            }
                        }
                    }
                    else if (action == "search" && request.QueryString["term"] != null)
                    {
                        // GET /api/books/search?term=xyz&by=title - Search books
                        var searchTerm = request.QueryString["term"];
                        var searchBy = request.QueryString["by"] ?? "all";
                        var books = await bookService.SearchBooksAsync(searchTerm, searchBy);
                        jsonResponse = JsonSerializer.Serialize(books);
                    }
                    else if (action == "popular")
                    {
                        // GET /api/books/popular?count=5 - Get popular books
                        int count = 5;
                        if (request.QueryString["count"] != null && int.TryParse(request.QueryString["count"], out int customCount))
                        {
                            count = customCount;
                        }
                        var books = await bookService.GetPopularBooksAsync(count);
                        jsonResponse = JsonSerializer.Serialize(books);
                    }
                    else if (action == "related" && request.QueryString["id"] != null)
                    {
                        // GET /api/books/related?id=xyz - Get related books
                        if (Guid.TryParse(request.QueryString["id"], out Guid bookId))
                        {
                            var books = await bookService.GetRelatedBooksAsync(bookId);
                            jsonResponse = JsonSerializer.Serialize(books);
                        }
                        else
                        {
                            response.StatusCode = 400; // Bad Request
                            jsonResponse = "{\"error\": \"Invalid book ID\"}";
                        }
                    }
                    else if (Guid.TryParse(action, out Guid bookId))
                    {
                        // GET /api/books/{id} - Get book by ID
                        if (request.HttpMethod == "GET")
                        {
                            var book = await bookService.GetBookByIdAsync(bookId);
                            if (book != null)
                                jsonResponse = JsonSerializer.Serialize(book);
                            else
                            {
                                response.StatusCode = 404; // Not Found
                                jsonResponse = "{\"error\": \"Book not found\"}";
                            }
                        }
                        else if (request.HttpMethod == "PUT")
                        {
                            // PUT /api/books/{id} - Update book
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string requestBody = await reader.ReadToEndAsync();
                                var book = JsonSerializer.Deserialize<Book>(requestBody);
                                
                                if (book.Id != bookId)
                                {
                                    response.StatusCode = 400; // Bad Request
                                    jsonResponse = "{\"error\": \"Book ID mismatch\"}";
                                }
                                else
                                {
                                    await bookService.UpdateBookAsync(book);
                                    jsonResponse = JsonSerializer.Serialize(book);
                                }
                            }
                        }
                        else if (request.HttpMethod == "DELETE")
                        {
                            // DELETE /api/books/{id} - Delete book
                            await bookService.DeleteBookAsync(bookId);
                            response.StatusCode = 204; // No Content
                        }
                    }
                }
                else if (controller == "loans")
                {
                    if (string.IsNullOrEmpty(action))
                    {
                        // GET /api/loans - Get all loans
                        var loans = await loanService.GetAllLoansAsync();
                        jsonResponse = JsonSerializer.Serialize(loans);
                    }
                    else if (action == "active")
                    {
                        // GET /api/loans/active - Get active loans
                        var loans = await loanService.GetActiveLoansAsync();
                        jsonResponse = JsonSerializer.Serialize(loans);
                    }
                    else if (action == "overdue")
                    {
                        // GET /api/loans/overdue - Get overdue loans
                        var loans = await loanService.GetOverdueLoansAsync();
                        jsonResponse = JsonSerializer.Serialize(loans);
                    }
                    else if (action == "borrow" && request.HttpMethod == "POST")
                    {
                        // POST /api/loans/borrow - Borrow a book
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = await reader.ReadToEndAsync();
                            var borrowRequest = JsonSerializer.Deserialize<BorrowRequest>(requestBody);
                            
                            var result = await loanService.BorrowBookAsync(
                                borrowRequest.BookId,
                                borrowRequest.PatronName,
                                borrowRequest.PatronEmail,
                                borrowRequest.LoanDays
                            );
                            
                            if (result.Success)
                            {
                                response.StatusCode = 201; // Created
                                jsonResponse = JsonSerializer.Serialize(new { message = result.Message });
                            }
                            else
                            {
                                response.StatusCode = 400; // Bad Request
                                jsonResponse = JsonSerializer.Serialize(new { error = result.Message });
                            }
                        }
                    }
                    else if (action == "return" && request.HttpMethod == "POST")
                    {
                        // POST /api/loans/return - Return a book
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = await reader.ReadToEndAsync();
                            var returnRequest = JsonSerializer.Deserialize<ReturnRequest>(requestBody);
                            
                            var result = await loanService.ReturnBookAsync(returnRequest.LoanId);
                            
                            if (result.Success)
                            {
                                jsonResponse = JsonSerializer.Serialize(new { message = result.Message });
                            }
                            else
                            {
                                response.StatusCode = 400; // Bad Request
                                jsonResponse = JsonSerializer.Serialize(new { error = result.Message });
                            }
                        }
                    }
                }
                else if (controller == "notifications")
                {
                    if (string.IsNullOrEmpty(action))
                    {
                        // GET /api/notifications - Get all notifications
                        var notifications = notificationService.GetPendingNotifications();
                        jsonResponse = JsonSerializer.Serialize(notifications);
                    }
                    else if (action == "send" && request.HttpMethod == "POST")
                    {
                        // POST /api/notifications/send - Send all notifications
                        notificationService.SendNotifications();
                        jsonResponse = JsonSerializer.Serialize(new { message = "All notifications have been sent." });
                    }
                }
                
                // Write the JSON response
                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(jsonResponse);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal Server Error
                byte[] buffer = Encoding.UTF8.GetBytes($"{{\"error\": \"{ex.Message}\"}}");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }

    // Helper classes for API requests
    class BorrowRequest
    {
        public Guid BookId { get; set; }
        public string PatronName { get; set; }
        public string PatronEmail { get; set; }
        public int LoanDays { get; set; } = 14;
    }

    class ReturnRequest
    {
        public Guid LoanId { get; set; }
    }
}