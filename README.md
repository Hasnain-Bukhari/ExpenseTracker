# Expense Tracker

A comprehensive expense tracking application built with modern web technologies to help users manage their finances effectively.

## 🏗️ Architecture

This project consists of two main components:

- **Backend API** (`ExpenseTrackerAPI/`) - .NET 8 Web API with PostgreSQL database
- **Frontend UI** (`ExpenseTrackerUI/`) - Vue.js 3 application with Vuetify UI framework

## 🚀 Features

### Core Functionality
- **User Authentication** - Secure JWT-based authentication system
- **Account Management** - Create and manage multiple financial accounts
- **Category Management** - Organize expenses and income with categories and subcategories
- **Currency Support** - Multi-currency support for global users
- **Account Types** - Different account types (Bank, Credit Card, etc.)

### User Interface
- **Modern Design** - Clean, responsive UI built with Vuetify
- **Dark/Light Theme** - User preference-based theme switching
- **Real-time Updates** - Live data synchronization
- **Mobile Responsive** - Optimized for all device sizes

## 🛠️ Technology Stack

### Backend
- **.NET 8** - Modern C# web framework
- **ASP.NET Core** - Web API framework
- **NHibernate** - Object-relational mapping
- **PostgreSQL** - Primary database
- **JWT Authentication** - Secure token-based auth
- **FluentValidation** - Input validation

### Frontend
- **Vue.js 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vuetify 3** - Material Design component framework
- **Vite** - Fast build tool and dev server
- **Pinia** - State management
- **Vue Router** - Client-side routing
- **Axios** - HTTP client

## 📁 Project Structure

```
ExpenseTracker/
├── docker-compose.yml              # Docker orchestration
├── docker-manage.sh               # Docker management script
├── ExpenseTrackerAPI/              # Backend API
│   ├── src/
│   │   ├── ExpenseTracker.API/     # Web API controllers
│   │   │   └── Dockerfile          # API container definition
│   │   ├── ExpenseTracker.Service/ # Business logic
│   │   ├── ExpenseTracker.Repository/ # Data access
│   │   └── ExpenseTracker.Dtos/    # Data transfer objects
│   ├── .dockerignore               # Docker build exclusions
│   └── README.md                   # API documentation
├── ExpenseTrackerUI/               # Frontend application
│   ├── src/
│   │   ├── components/             # Vue components
│   │   ├── views/                 # Page components
│   │   ├── stores/                # Pinia stores
│   │   ├── types/                  # TypeScript definitions
│   │   └── lib/                   # Utilities and API client
│   ├── Dockerfile                  # UI container definition
│   ├── nginx.conf                  # Nginx configuration
│   ├── .dockerignore              # Docker build exclusions
│   ├── .env                        # Local environment
│   ├── .env.docker                 # Docker environment
│   └── README.md                   # UI documentation
└── README.md                       # This file
```

## 🚀 Quick Start

### Option 1: Docker (Recommended)
The easiest way to run the entire application locally:

```bash
# Start all services with one command
./docker-manage.sh start
```

**Access your application:**
- **Frontend**: http://localhost:3000
- **API**: http://localhost:5001
- **Database**: localhost:5432

**Other Docker commands:**
```bash
./docker-manage.sh stop      # Stop all services
./docker-manage.sh logs      # View logs
./docker-manage.sh status    # Check status
./docker-manage.sh cleanup   # Remove everything
```

### Option 2: Manual Setup

#### Prerequisites
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 15+

#### Backend Setup
```bash
cd ExpenseTrackerAPI
dotnet restore
dotnet run --project src/ExpenseTracker.API
```

#### Frontend Setup
```bash
cd ExpenseTrackerUI
npm install
npm run dev
```

#### Database Setup
1. Create a PostgreSQL database
2. Update connection string in `ExpenseTrackerAPI/src/ExpenseTracker.API/Program.cs`
3. Run the application to auto-create tables

## 🔧 Development

### Backend Development
- Controllers handle HTTP requests
- Services contain business logic
- Repositories manage data access
- DTOs define API contracts

### Frontend Development
- Components are organized by feature
- Stores manage application state
- Types ensure type safety
- API client handles backend communication

## 📚 Documentation

- [API Documentation](./ExpenseTrackerAPI/README.md) - Detailed backend documentation
- [UI Documentation](./ExpenseTrackerUI/README.md) - Frontend development guide

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the documentation in each component folder
- Review the code comments for implementation details

---

**Built with ❤️ using modern web technologies**
