# Customer Management System

A modern desktop application built with Avalonia UI and .NET 6 for managing customer information. The application provides a user-friendly interface for creating, reading, updating, and deleting customer records with category management capabilities.

## Features

- 🏢 Customer Management (CRUD operations)
- 📁 Category Management
- 🔍 Advanced Search & Filtering
  - Search by customer name, code, email, or phone number
  - Filter customers by category
- 💻 Modern UI with Avalonia
- 🎨 Fluent Design System
- 🔒 Data Validation
- 🗃️ SQL Server Database Integration

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later
- Docker (optional, for running SQL Server in a container)

## Getting Started

1. Clone the repository:

```bash
git clone https://github.com/yourusername/customer-management.git
cd customer-management
cd CustomerManagement
```

2. Start SQL Server (if using Docker):

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sql1 --hostname sql1 \
   -d mcr.microsoft.com/mssql/server:2019-latest
```

3. Update the connection string in `App.axaml.cs` if needed:

```csharp
options.UseSqlServer("Server=localhost,1433;Database=CustomerManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");
```

4. Apply database migrations:

```bash
dotnet ef database update
```

5. Build and run the application:

```bash
dotnet build
dotnet run
```

## Project Structure

```
CustomerManagement/
├── Data/                  # Database context and configurations
├── Models/               # Domain models
├── Services/            # Business logic and data access
├── ViewModels/          # MVVM view models
├── Views/               # Avalonia UI views
└── App.axaml           # Application entry point and styling
```

## Key Components

- **Models**: Contains the domain entities like `Customer` and `Category`
- **ViewModels**: Implements the MVVM pattern for UI logic
- **Services**: Handles data operations and business logic
- **Views**: Avalonia UI components for the user interface

## Database Schema

The application uses two main tables:

- **Customers**: Stores customer information
- **Categories**: Stores customer categories

## Features in Detail

### Customer Management

- Add new customers with validation
- Edit existing customer information
- Delete customers
- View customer details

### Category Management

- Predefined categories (Customer, Supplier, Distributor, VIP)
- Add new categories
- Assign categories to customers

### Search and Filter

- Real-time search functionality
- Filter by multiple fields
- Category-based filtering

