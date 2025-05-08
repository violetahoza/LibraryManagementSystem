# Library Management System

## Overview

This Library Management System is a robust application designed for library administrators to efficiently manage book collections. The system allows adding new titles, removing worn-out ones, and facilitating the borrowing process. It features both a console interface and a simple web interface.

## Features

- **Multi-layered Architecture**: Follows a modular design for better maintainability and scalability
- **Dual Interface**: Console UI and Web UI options
- **Book Management**: Add, edit, delete, and search for books
- **Loan Management**: Process book borrowing and returns, track due dates
- **Search Functionality**: Find books using various criteria (title, author, genre, etc.)
- **Smart Recommendation System**: View popular books and find related titles
- **Notifications**: Automated notifications for overdue books and recommendations

## Requirements

- **.NET 8.0 SDK** or newer
- Any modern web browser (for Web UI)

## Installation

1. Clone or download the repository
2. Navigate to the project directory
3. Build the project: dotnet build

## Usage

1. Run the application: dotnet run
2. Choose interface option:
- Option 1: Console UI
- Option 2: Web UI

3. If using Web UI, open your browser and navigate to: http://localhost:8080/

   ## Console UI

The console interface provides a menu-driven experience for:
- Managing books
- Processing loans
- Searching books
- Viewing recommendations
- Managing notifications

## Web UI

The web interface offers a user-friendly way to interact with the system:
- Dashboard with statistics
- Book management
- Loan processing
- Advanced search
- Recommendation system
- Notification management

## Data Storage

The system uses JSON files for data persistence:
- `books.json`: Stores book information
- `loans.json`: Stores loan records

Files are stored in the `Data` folder of the application directory.

## Smart Recommendation System

This innovative feature enhances the user experience by:
- Tracking book popularity (borrow count)
- Finding related books based on author, genre, and tags
- Providing personalized recommendations
- Automating notifications about available and related books

## Credits

Developed for the Siemens .NET Internship 2025.
