# ClipVault - Code Snippet Manager with Tagging

ClipVault is a full-stack application designed to help developers organize, search, and manage their code snippets. It supports syntax highlighting, tagging, search functionality, and optional AI-powered tag suggestions.

## Features

### Core Features
- **User Authentication**: Secure user registration and login using JWT-based authentication with HTTP interceptor and route guards.
- **Snippet Management**: Create, edit, delete, and view code snippets with full CRUD operations.
- **Collections Organization**: Organize snippets into collections with hierarchical structure.
- **Monaco Code Editor**: Advanced code editor with syntax highlighting, language detection, and Monaco Editor integration.
- **Tagging System**: Add and manage tags for snippets with a many-to-many relationship.
- **Search and Filter**: Search snippets by language, tag, or keyword.
- **Syntax Highlighting**: View code snippets with syntax highlighting.
- **Responsive UI**: Fully responsive design with dark mode support.

### Upcoming Features
- **AI Tagging**: Automatically generate tag suggestions using OpenAI or local logic.
- **Export Options**: Export snippets to Markdown or PDF.
- **Snippet Organization**: Group snippets by project or folder.

---

## Tech Stack

### Frontend
- **Framework**: Angular 20
- **Styling**: Angular Material, SCSS
- **Syntax Highlighting**: ngx-highlightjs

### Backend
- **Framework**: ASP.NET Core 9 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT-based authentication

---

## Prerequisites

Before running ClipVault, ensure you have the following installed:

- **Node.js** (v18 or higher)
- **npm** (v9 or higher)
- **.NET 9 SDK**
- **PostgreSQL** (v13 or higher)
- **Git**

---

## Installation & Setup

### 1. Clone the Repository

```powershell
git clone https://github.com/your-username/ClipVault.git
cd ClipVault
```

### 2. Database Setup

#### Install PostgreSQL
1. Download and install PostgreSQL from [postgresql.org](https://www.postgresql.org/download/)
2. Create a new database for ClipVault:

```sql
-- Connect to PostgreSQL as superuser
CREATE DATABASE clipvault_db;
CREATE USER clipvault_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE clipvault_db TO clipvault_user;
```

### 3. Backend Setup

#### Navigate to backend directory
```powershell
cd clipvault-backend
```

#### Environment Configuration
1. Create environment files:
   - Copy `appsettings.Development.json.example` to `appsettings.Development.json`
   - Update the connection string and JWT settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=clipvault_db;Username=clipvault_user;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-here",
    "Issuer": "ClipVault",
    "Audience": "ClipVault-Users",
    "ExpiryInMinutes": 60
  }
}
```

#### Install dependencies and run migrations
```powershell
# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the backend
dotnet run
```

The backend will be available at `http://localhost:5157`

### 4. Frontend Setup

#### Navigate to frontend directory
```powershell
cd ../clipvault-frontend
```

#### Environment Configuration
1. Create environment files in `src/env/`:

**src/env/env.ts** (Development)
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5157',
  appName: 'ClipVault'
};
```

**src/env/env.prod.ts** (Production)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-production-api.com',
  appName: 'ClipVault'
};
```

#### Install dependencies and run
```powershell
# Install npm packages
npm install

# Start development server
npm start
```

The frontend will be available at `http://localhost:4200`

---

## API Documentation


### Using Swagger
The Swagger interface provides:
- Complete API endpoint documentation
- Interactive testing of all endpoints
- Request/response schemas
- Authentication testing with JWT tokens
- Real-time API exploration

To access Swagger:
1. Start the backend server (`dotnet run`)
2. Navigate to `http://localhost:5157/swagger` in your browser
3. Use the "Authorize" button to test authenticated endpoints with your JWT token


### Frontend (`src/env/`)
Create these files and add them to `.gitignore`:

- `env.ts` - Development environment
- `env.prod.ts` - Production environment
- `env.staging.ts` - Staging environment (optional)

---

## Development Commands

### Backend
```powershell
# Run in development mode
dotnet run

# Run with hot reload
dotnet watch run

# Run tests
dotnet test

# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Build for production
dotnet build --configuration Release
```

### Frontend
```powershell
# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test

# Run linting
npm run lint

# Update dependencies
npm update
```