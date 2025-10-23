# Expense Tracker API

A robust .NET 8 Web API backend for the Expense Tracker application, providing secure and scalable financial data management.

## 🏗️ Architecture

The API follows a clean architecture pattern with clear separation of concerns:

- **Controllers** - Handle HTTP requests and responses
- **Services** - Contain business logic and validation
- **Repositories** - Manage data access and persistence
- **DTOs** - Define data transfer contracts
- **Models** - Core domain entities

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **NHibernate** - Object-relational mapping
- **PostgreSQL** - Primary database
- **JWT Authentication** - Secure token-based authentication
- **FluentValidation** - Input validation and business rules
- **Swagger/OpenAPI** - API documentation

## 📁 Project Structure

```
ExpenseTrackerAPI/
├── src/
│   ├── ExpenseTracker.API/           # Web API layer
│   │   ├── Controllers/              # API endpoints
│   │   │   ├── AuthController.cs     # Authentication endpoints
│   │   │   ├── AccountsController.cs # Account management
│   │   │   ├── CategoriesController.cs # Category management
│   │   │   ├── CurrenciesController.cs # Currency management
│   │   │   └── AccountTypesController.cs # Account type management
│   │   ├── Program.cs               # Application configuration
│   │   └── Properties/              # Launch settings
│   ├── ExpenseTracker.Service/       # Business logic layer
│   │   ├── Services/                # Service implementations
│   │   │   ├── Auth/                # Authentication services
│   │   │   ├── AccountService.cs    # Account business logic
│   │   │   ├── CategoryService.cs   # Category business logic
│   │   │   ├── CurrencyService.cs  # Currency business logic
│   │   │   └── AccountTypeService.cs # Account type business logic
│   │   └── Services.Auth/Validators/ # Input validation
│   ├── ExpenseTracker.Repository/    # Data access layer
│   │   ├── Repositories/            # Repository implementations
│   │   │   ├── NativeUserRepository.cs
│   │   │   ├── NativeAccountRepository.cs
│   │   │   ├── NativeCategoryRepository.cs
│   │   │   ├── NativeCurrencyRepository.cs
│   │   │   └── NativeAccountTypeRepository.cs
│   │   └── Mapping/                 # NHibernate mappings
│   │       ├── AuthMappings.hbm.xml
│   │       ├── Entities.hbm.xml
│   │       ├── AccountMappings.hbm.xml
│   │       ├── CategoryMappings.hbm.xml
│   │       ├── CurrencyMappings.hbm.xml
│   │       └── AccountTypeMappings.hbm.xml
│   └── ExpenseTracker.Dtos/         # Data transfer objects
│       ├── Auth/                    # Authentication DTOs
│       ├── Accounts/                # Account DTOs
│       ├── Categories/              # Category DTOs
│       ├── Currencies/              # Currency DTOs
│       ├── AccountTypes/            # Account type DTOs
│       └── Models/                  # Core domain models
└── ExpenseTracker.sln              # Solution file
```

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ExpenseTracker/ExpenseTrackerAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database**
   - Create a PostgreSQL database
   - Update connection string in `Program.cs`:
   ```csharp
   string usedConnectionString = "Host=localhost;Port=5432;Database=expense_tracker;Username=your_user;Password=your_password;";
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/ExpenseTracker.API
   ```

5. **Access Swagger documentation**
   - Navigate to `http://localhost:5001/swagger`

### Docker Setup (Alternative)

For easier setup and consistent environment:

1. **Using Docker Compose (from project root)**
   ```bash
   cd /path/to/ExpenseTracker
   ./docker-manage.sh start
   ```

2. **Manual Docker build**
   ```bash
   # Build the API container
   docker build -f src/ExpenseTracker.API/Dockerfile -t expense-tracker-api .
   
   # Run with PostgreSQL
   docker run -d --name postgres -e POSTGRES_DB=expense_tracker_dev -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:15
   
   # Run the API
   docker run -d --name api -p 5001:80 --link postgres:postgres -e CONNECTION_STRING="Host=postgres;Port=5432;Database=expense_tracker_dev;Username=postgres;Password=postgres" expense-tracker-api
   ```

3. **Access the API**
   - API: `http://localhost:5001`
   - Swagger: `http://localhost:5001/swagger`

## 🔐 Authentication

The API uses JWT (JSON Web Token) authentication:

### Endpoints
- `POST /v1/auth/register` - User registration
- `POST /v1/auth/login` - User login
- `POST /v1/auth/refresh` - Token refresh
- `POST /v1/auth/forgot-password` - Password reset request
- `POST /v1/auth/reset-password` - Password reset

### Usage
Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## 📊 API Endpoints

### Accounts
- `GET /v1/accounts` - List user accounts
- `GET /v1/accounts/{id}` - Get account details
- `POST /v1/accounts` - Create new account
- `PUT /v1/accounts/{id}` - Update account
- `DELETE /v1/accounts/{id}` - Delete account

### Categories
- `GET /v1/categories` - List categories
- `GET /v1/categories/{id}` - Get category details
- `POST /v1/categories` - Create category
- `PUT /v1/categories/{id}` - Update category
- `DELETE /v1/categories/{id}` - Delete category

### Subcategories
- `GET /v1/categories/{id}/subcategories` - List subcategories
- `POST /v1/categories/{id}/subcategories` - Create subcategory
- `PUT /v1/subcategories/{id}` - Update subcategory
- `DELETE /v1/subcategories/{id}` - Delete subcategory

### Currencies
- `GET /v1/currencies` - List currencies
- `GET /v1/currencies/{id}` - Get currency details
- `POST /v1/currencies` - Create currency
- `PUT /v1/currencies/{id}` - Update currency
- `DELETE /v1/currencies/{id}` - Delete currency

### Account Types
- `GET /v1/account-types` - List account types
- `GET /v1/account-types/{id}` - Get account type details
- `POST /v1/account-types` - Create account type
- `PUT /v1/account-types/{id}` - Update account type
- `DELETE /v1/account-types/{id}` - Delete account type

## 🗄️ Database Schema

### Core Tables
- **users** - User accounts and profiles
- **accounts** - Financial accounts
- **categories** - Expense/income categories
- **subcategories** - Category subdivisions
- **currencies** - Supported currencies
- **account_types** - Account classifications

### Relationships
- Users have many Accounts
- Accounts belong to AccountType and Currency
- Categories have many Subcategories
- All entities include audit fields (CreatedAt, UpdatedAt)

## 🔧 Configuration

### Environment Variables
- `CONNECTION_STRING` - Database connection string
- `JWT_SECRET` - JWT signing secret
- `FRONTEND_ORIGINS` - CORS allowed origins
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)

### NHibernate Configuration
- Automatic table creation
- SQL logging enabled in development
- Optimized queries with lazy loading
- Transaction management

## 🧪 Testing

### Manual Testing
Use Swagger UI at `/swagger` to test endpoints interactively.

### Example API Calls

**Register User:**
```bash
curl -X POST "http://localhost:5001/v1/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

**Login:**
```bash
curl -X POST "http://localhost:5001/v1/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!"
  }'
```

**Create Account:**
```bash
curl -X POST "http://localhost:5001/v1/accounts" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "My Checking Account",
    "accountTypeId": "account-type-guid",
    "currencyId": "currency-guid",
    "isSavings": false,
    "openingBalance": 1000.00,
    "includeInNetworth": true
  }'
```

## 🚀 Deployment

### Production Considerations
1. **Security**
   - Use strong JWT secrets
   - Enable HTTPS
   - Configure proper CORS policies
   - Use environment-specific connection strings

2. **Performance**
   - Enable connection pooling
   - Configure NHibernate caching
   - Use production database settings

3. **Monitoring**
   - Add logging framework (Serilog)
   - Implement health checks
   - Add application metrics

## 🔍 Troubleshooting

### Common Issues

**Database Connection Issues:**
- Verify PostgreSQL is running
- Check connection string format
- Ensure database exists

**Authentication Errors:**
- Verify JWT secret is set
- Check token expiration
- Ensure proper Authorization header format

**NHibernate Mapping Errors:**
- Check mapping file syntax
- Verify entity property names
- Ensure proper assembly references

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [NHibernate Documentation](https://nhibernate.info/)
- [JWT Authentication Guide](https://jwt.io/introduction/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

**Built with .NET 8 and modern C# practices**
