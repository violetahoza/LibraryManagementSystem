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
```
## Configuration
The application uses JSON files to store data, which are saved in a "Data" directory:
* `books.json`: Stores information about all books
* `loans.json`: Stores records of all book loans

The application will automatically:
1. Create the Data directory if it doesn't exist
2. Create empty JSON files if they don't exist
3. Load sample data on first run if the books repository is empty

## Using the Application
When you start the application, you'll see a menu with numbered options:

```bash
===== LIBRARY MANAGEMENT SYSTEM =====

1. View All Books
2. Add New Book
3. Update Book
4. Delete Book
5. Search Books
6. Lend Book
7. Return Book
8. Get Book Recommendations
9. Exit
```
### 1. View All Books
Displays a table with all books in the library, including their ID, title, author, ISBN, and availability status.

### 2. Add New Book
Prompts you to enter details for a new book:
* Title
* Author
* ISBN
* Genre
* Publication Year
* Quantity

### 3. Update Book
1. Enter the ID of the book you want to update
2. Review current details
3. Enter new values for any fields you want to update (leave blank to keep current value)

### 4. Delete Book
1. Enter the ID of the book you want to delete
2. Confirm deletion
3. Note: Books with active loans cannot be deleted

### 5. Search Books
1. Choose a filter type:
   * Title
   * Author
   * ISBN
   * Genre
   * Available Books Only
   * All Fields
2. Enter your search term (if applicable)
3. View results

### 6. Lend Book
1. Enter the ID of the book to lend
2. Enter the borrower's name
3. The system will update availability and create a loan record

### 7. Return Book
1. Enter the ID of the book being returned
2. Enter the borrower's name
3. The system will validate the loan and update records

### 8. Get Book Recommendations
1. Enter a borrower's name
2. The system will analyze their borrowing history and recommend similar books based on genre and author preferences. Here's how it works:
- The system analyzes the borrower's past loan records to identify:
   * Genres they have shown interest in
   * Authors they have previously read
- It then identifies available books that match these preferences but that the borrower hasn't read yet.
- The system ranks and presents up to five recommended books that align with the borrower's reading preferences.

This feature helps enhance the user experience by:
* Encouraging exploration of new books within familiar genres
* Introducing readers to more works by authors they enjoy
* Increasing circulation of books by matching them with interested readers
