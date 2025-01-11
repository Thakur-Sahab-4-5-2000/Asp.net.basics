# Learning Backend

This project is a backend for a learning platform, built using .NET, Entity Framework, and MySQL. It supports JWT authentication and provides endpoints for user and content management. The backend is designed for scalability, security, and ease of use.

## Features

- JWT-based Authentication
- MySQL Database Integration
- Entity Framework for ORM
- Swagger API Documentation
- Middleware for Authentication and Error Handling

## Technologies

- **.NET 6.0+**
- **Entity Framework Core**
- **MySQL**
- **JWT (JSON Web Tokens) Authentication**
- **Swagger for API Documentation**

## Installation

### Prerequisites

- .NET 6.0 or higher
- MySQL Server
- Visual Studio or your preferred IDE

### Steps

1. Clone the repository:

    ```bash
    git clone https://github.com/yourusername/Learning-Backend.git
    ```

2. Navigate to the project directory:

    ```bash
    cd Learning-Backend
    ```

3. Install the dependencies:

    ```bash
    dotnet restore
    ```

4. Update your `appsettings.json` with your MySQL connection string:

    ```json
    {
        "ConnectionStrings": {
            "DefaultConnection": "server=your-server;database=your-db;user=your-user;password=your-password"
        },
        "JWT_KEYS": "your-secret-key"
    }
    ```

5. Apply the database migrations:

    ```bash
    dotnet ef database update
    ```

6. Run the application:

    ```bash
    dotnet run
    ```

    The server will start, and you can access the API at `https://localhost:5001`.

## API Documentation

You can view and test the API using Swagger. Once the application is running, visit `https://localhost:5001/swagger` to interact with the API documentation.

## Gitignore

This project uses `.gitignore` to exclude unnecessary files from being tracked by Git. Make sure to add `.vs/` and other IDE-specific files to your `.gitignore`:

```gitignore
.vs/
bin/
obj/
*.user
*.suo
*.db
