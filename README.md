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
- Docker Desktop
- Git

## Detailed Installation Steps

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/kgnylm/customer-management.git
   cd customer-management
   ```

2. **Start SQL Server with Docker:**

   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sql1 --hostname sql1 \
   -d mcr.microsoft.com/mssql/server:2019-latest
   ```

   > Note: Wait about 10-15 seconds for SQL Server to fully start

3. **Install Entity Framework Tools:**

   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Navigate to Project Directory and Restore Dependencies:**

   ```bash
   cd CustomerManagement
   dotnet restore
   ```

5. **Create Database and Apply Migrations:**

   ```bash
   dotnet ef database update
   ```

6. **Run the Application:**
   ```bash
   dotnet run
   ```

## Troubleshooting

### Common Issues

1. **Docker Container Issues:**

   - If Docker was closed, restart Docker Desktop and run:
     ```bash
     docker start sql1
     ```
   - Wait 10-15 seconds before starting the application

2. **Database Connection Issues:**

   - Ensure SQL Server is running:
     ```bash
     docker ps | grep sql1
     ```
   - Check if the database exists:
     ```bash
     dotnet ef database update
     ```

3. **Entity Framework Tools Missing:**
   - If you get "dotnet ef not found" error, run:
     ```bash
     dotnet tool install --global dotnet-ef
     ```

### Connection String

The default connection string is:

```
Server=localhost,1433;Database=CustomerManagement;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
```

You can modify this in `App.axaml.cs` if needed.

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
