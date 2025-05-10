# Library Management System

This is a console-based Library Management System developed in C# that allows efficient management of books, including lending, returning, and providing personalized book recommendations.

## Features

1. **View All Books**: Display a comprehensive list of all books in the library.
2. **Add New Book**: Add a new book to the library with details such as title, author, ISBN, etc.
3. **Update Book**: Modify existing book information.
4. **Delete Book**: Remove a book from the library inventory.
5. **Search Books**: Find books using various criteria like title, author, or genre.
6. **Lend Book**: Record when a book is borrowed by a user.
7. **Return Book**: Process book returns.
8. **Book Recommendations**: Get personalized book recommendations based on a borrower's reading history.

## System Requirements

- .NET 6.0 SDK or higher
- Windows, macOS, or Linux operating system

## Project Structure

The application follows a clean architecture with separation of concerns:

- **Models**: Book and LoanRecord classes
- **Interfaces**: IRepository and IBookService interfaces
- **Data**: JsonRepository for data persistence
- **Services**: BookService for business logic implementation
- **Program**: Main entry point with UI logic

## Installation and Setup

### Option 1: Using Visual Studio

1. Clone or download the repository to your local machine.
2. Open the solution file (.sln) in Visual Studio.
3. Restore NuGet packages (should happen automatically).
4. Build the solution (Ctrl+Shift+B).
5. Run the application (F5 or Ctrl+F5).

### Option 2: Using Command Line

1. Clone or download the repository to your local machine.
2. Open a terminal or command prompt.
3. Navigate to the project directory containing the .csproj file.
4. Run the following commands:

```bash
# Restore dependencies
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run
