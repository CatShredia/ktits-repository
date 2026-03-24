# CinemaProject - Cinema Catalog Web Application

A full-stack web application for managing and browsing a film catalog with user authentication, ratings, and role-based access control.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [User Roles](#user-roles)
- [Screenshots & UI](#screenshots--ui)
- [Development](#development)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)
- [License](#license)

---

## Overview

CinemaProject is a comprehensive cinema catalog system that allows users to browse films, rate movies, and manage film collections. Administrators have full control over the catalog, including films, genres, users, and ratings.

The application demonstrates modern .NET development practices with:
- RESTful API architecture
- JWT-based authentication
- Role-based authorization
- Clean separation of concerns
- Responsive Blazor WebAssembly frontend

---

## Features

### Public Features
- **Film Catalog** - Browse all films with cover images, descriptions, and ratings
- **Search & Filter** - Search films by name, filter by genre
- **Sorting** - Sort by name, release date, or rating (ascending/descending)
- **Film Details** - View detailed information including average rating and number of ratings
- **Genre Listing** - Browse available film genres

### Registered User Features
- **User Registration** - Create account with automatic "client" role assignment
- **Authentication** - Secure login with JWT tokens (7-day expiration)
- **Profile Management** - View and edit personal profile information
- **Rate Films** - Submit ratings (1-10 scale) for any film
- **Manage Own Ratings** - Edit or delete your own ratings
- **One Rating Per Film** - Each user can rate a film once

### Administrator Features
- **Film Management**
  - Create new films with title, description, release date, genre
  - Upload film poster images (JPEG, PNG, WebP, GIF; max 5MB)
  - Support for external image URLs
  - Edit and delete films
  - View "My Films" - films authored by the admin

- **Genre Management**
  - Create, edit, and delete film genres
  - Manage genre catalog

- **User Management**
  - View all users with search and filtering
  - Create new users with role assignment
  - Edit user profiles and credentials
  - Delete users and their login credentials
  - Filter users by role

- **Rating Management**
  - View all ratings across the system
  - Update or delete any user's rating

---

## Technology Stack

### Backend (CinemaAPI)

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Runtime framework |
| ASP.NET Core Web API | 8.0 | REST API framework |
| Entity Framework Core | 8.0.24 | ORM |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.11 | PostgreSQL provider |
| Microsoft.AspNetCore.Authentication.JwtBearer | - | JWT authentication |
| System.IdentityModel.Tokens.Jwt | 8.16.0 | JWT token handling |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger/OpenAPI documentation |

### Frontend (CinemaBlazor)

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Runtime framework |
| Blazor WebAssembly | 8.0.25 | SPA framework |
| Microsoft.AspNetCore.Components.Authorization | - | Auth state management |
| System.Net.Http.Json | - | HTTP client extensions |
| Bootstrap | 5.x | UI styling |
| Bootstrap Icons | - | Icon library |

### Database

| Technology | Version |
|------------|---------|
| PostgreSQL | 15+ (recommended) |
| Database Name | `cinemaDB` |

---

## Project Structure

```
CinemaProject/
├── CinemaAPI/                    # Backend Web API
│   ├── Controllers/              # REST API endpoints
│   │   ├── AuthController.cs     # Registration, login, profile
│   │   ├── FilmsController.cs    # Film CRUD operations
│   │   ├── GenresController.cs   # Genre CRUD operations
│   │   ├── RatingsController.cs  # Rating CRUD operations
│   │   ├── UsersController.cs    # User management (admin)
│   │   └── RolesController.cs    # Role management
│   ├── Models/                   # Database entities
│   │   ├── User.cs
│   │   ├── Login.cs
│   │   ├── Role.cs
│   │   ├── Film.cs
│   │   ├── Genre.cs
│   │   └── Rating.cs
│   ├── Data/                     # Database context
│   │   └── CinemaDbContext.cs
│   ├── Services/                 # Business logic
│   │   └── ImageService.cs       # Image upload handling
│   ├── DTOs/                     # Data transfer objects
│   │   ├── RegisterDto.cs
│   │   ├── LoginDto.cs
│   │   └── FilmDto.cs
│   ├── wwwroot/images/films/     # Uploaded film images
│   ├── appsettings.json          # Configuration
│   └── Program.cs                # Application entry point
│
├── CinemaBlazor/                 # Frontend Blazor WASM
│   ├── Pages/                    # Razor pages
│   │   ├── Home.razor
│   │   ├── Login.razor
│   │   ├── Register.razor
│   │   ├── Profile.razor
│   │   ├── ProfileEdit.razor
│   │   ├── Films/
│   │   │   ├── FilmsList.razor
│   │   │   ├── FilmDetails.razor
│   │   │   ├── FilmCreate.razor
│   │   │   ├── FilmEdit.razor
│   │   │   └── MyFilms.razor
│   │   ├── Genres/
│   │   │   ├── GenresList.razor
│   │   │   ├── GenreCreate.razor
│   │   │   └── GenreEdit.razor
│   │   ├── Ratings/
│   │   │   └── RatingsList.razor  # Manage own ratings (edit/delete)
│   │   └── Users/
│   │       ├── UsersList.razor
│   │       ├── UserDetails.razor
│   │       ├── UserCreate.razor
│   │       ├── UserEdit.razor
│   │       └── UserLoginEdit.razor
│   ├── Services/                 # API client services
│   │   ├── AuthService.cs
│   │   ├── FilmService.cs
│   │   ├── GenreService.cs
│   │   ├── RatingService.cs
│   │   ├── UserService.cs
│   │   ├── LocalStorageService.cs
│   │   └── ApiUrlService.cs
│   ├── Models/                   # Client-side models
│   ├── Layout/                   # Shared layouts
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── wwwroot/                  # Static assets
│
├── insert_sql.sql                # Database schema + sample data
└── CinemaProject.slnx            # Solution file
```

---

## Database Schema

### Entity Relationship Diagram

```
┌─────────────┐       ┌─────────────┐
│   Roles     │       │   Genres    │
├─────────────┤       ├─────────────┤
│ Id          │       │ Id          │
│ Name        │       │ Name        │
└──────┬──────┘       └──────┬──────┘
       │                     │
       │ 1:N                 │ 1:N
       ▼                     ▼
┌─────────────┐       ┌─────────────┐
│   Users     │       │   Films     │
├─────────────┤       ├─────────────┤
│ Id          │◄──────│ Id          │
│ Surname     │       │ Name        │
│ Name        │       │ Description │
│ Email       │       │ ReleaseDate │
│ Gender      │       │ ImageUrl    │
│ Description │       │ GenreId     │
│ RoleId      │──────►│ AuthorId    │
└──────┬──────┘       └──────┬──────┘
       │                     │
       │ 1:1                 │ 1:N
       ▼                     ▼
┌─────────────┐       ┌─────────────┐
│   Logins    │       │  Ratings    │
├─────────────┤       ├─────────────┤
│ Id          │       │ Id          │
│ LoginValue  │       │ Value       │
│ PasswordHash│       │ FilmId      │
│ UserId      │──────►│ AuthorId    │
└─────────────┘       └─────────────┘
```

### Tables Description

| Table | Columns | Description |
|-------|---------|-------------|
| **Roles** | Id, Name | User roles (admin, client) |
| **Users** | Id, Surname, Name, Email, Gender, Description, RoleId | User profiles |
| **Logins** | Id, LoginValue, PasswordHash, UserId | Authentication credentials (1:1 with Users) |
| **Genres** | Id, Name | Film genres |
| **Films** | Id, Name, Description, ReleaseDate, ImageUrl, GenreId, AuthorId | Film catalog |
| **Ratings** | Id, Value (1-10), FilmId, AuthorId | User ratings for films |

### Relationships

- **Users → Roles**: Many-to-One (SET NULL on delete)
- **Users → Logins**: One-to-One (CASCADE on delete)
- **Users → Films**: One-to-Many (SET NULL on delete)
- **Users → Ratings**: One-to-Many (SET NULL on delete)
- **Genres → Films**: One-to-Many (SET NULL on delete)
- **Films → Ratings**: One-to-Many (CASCADE on delete)

---

## Installation & Setup

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd CinemaProject
```

### Step 2: Database Setup

1. **Create PostgreSQL Database**

```sql
CREATE DATABASE "cinemaDB";
```

2. **Run Schema and Seed Data**

Execute the `insert_sql.sql` file:

```bash
psql -U postgres -d cinemaDB -f insert_sql.sql
```

Or use pgAdmin/other PostgreSQL client to run the SQL file.

### Step 3: Configure the API

Edit `CinemaAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cinemaDB;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Issuer": "CinemaAPI",
    "Audience": "CinemaAPIUsers",
    "Key": "your_secret_key_at_least_32_chars"
  }
}
```

### Step 4: Build and Run

**Backend (CinemaAPI):**

```bash
cd CinemaAPI
dotnet restore
dotnet run
```

The API will start at `https://localhost:7156` and `http://localhost:5000`

**Frontend (CinemaBlazor):**

Open a new terminal:

```bash
cd CinemaBlazor
dotnet restore
dotnet run
```

The Blazor app will start at `https://localhost:7157`

### Step 5: Access the Application

Open your browser and navigate to:
- **Frontend:** `https://localhost:7157`
- **API Swagger:** `https://localhost:7156/swagger`

---

## Configuration

### appsettings.json (CinemaAPI)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cinemaDB;Username=postgres;Password=qwerty123"
  },
  "JwtSettings": {
    "Issuer": "CinemaAPI",
    "Audience": "CinemaAPIUsers",
    "Key": "sGfUT7LWQwU7TGB4aEHLDEKhFWst9wNh",
    "ExpiresInDays": 7
  },
  "AllowedHosts": "*",
  "CorsOrigins": [
    "https://localhost:5156",
    "https://localhost:7157"
  ],
  "ImageSettings": {
    "MaxSizeBytes": 5242880,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".webp", ".gif"],
    "UploadPath": "wwwroot/images/films"
  }
}
```

### Environment Variables (Optional)

For production deployments, consider using environment variables:

```bash
# Connection String
export ConnectionStrings__DefaultConnection="Host=...;Database=...;Username=...;Password=..."

# JWT Key
export JwtSettings__Key="your-production-secret-key"
```

---

## API Reference

### Base URL
```
https://localhost:7156/api
```

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/Auth/register` | Register new user | No |
| POST | `/Auth/login` | Login and get JWT token | No |
| GET | `/Auth/me` | Get current user profile | Yes |
| PUT | `/Auth/me` | Update current user profile | Yes |

### Films Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/Films` | Get all films (with search, filter, sort) | No | - |
| GET | `/Films/{id}` | Get film by ID | No | - |
| POST | `/Films` | Create new film | Yes | Admin |
| PUT | `/Films/{id}` | Update film | Yes | Admin |
| DELETE | `/Films/{id}` | Delete film | Yes | Admin |
| GET | `/Films/my-films` | Get films authored by current user | Yes | Admin |

**Query Parameters for GET /Films:**
- `search` - Search by film name
- `genreId` - Filter by genre
- `sortBy` - Sort field (name, releaseDate, rating)
- `isAscending` - Sort direction

### Genres Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/Genres` | Get all genres | No | - |
| GET | `/Genres/{id}` | Get genre by ID | No | - |
| POST | `/Genres` | Create new genre | Yes | Admin |
| PUT | `/Genres/{id}` | Update genre | Yes | Admin |
| DELETE | `/Genres/{id}` | Delete genre | Yes | Admin |

### Ratings Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/Ratings` | Get all ratings | Yes | Admin, Client |
| GET | `/Ratings/{id}` | Get rating by ID | Yes | Admin, Client |
| POST | `/Ratings` | Create new rating | Yes | Admin, Client |
| PUT | `/Ratings/{id}` | Update rating | Yes | Admin or owner |
| DELETE | `/Ratings/{id}` | Delete rating | Yes | Admin or owner |
| GET | `/Ratings/film/{filmId}/my-rating` | Get current user's rating for a film | Yes | Admin, Client |

**Note:** Users can update or delete only their own ratings. Admins can manage any rating.

### Users Endpoints (Admin Only)

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/Users` | Get all users (with search, filter) | Yes | Admin |
| GET | `/Users/{id}` | Get user by ID | Yes | Admin |
| POST | `/Users` | Create new user | Yes | Admin |
| PUT | `/Users/{id}` | Update user | Yes | Admin |
| DELETE | `/Users/{id}` | Delete user | Yes | Admin |
| GET | `/Users/{id}/login` | Get user's login credentials | Yes | Admin |
| PUT | `/Users/{userId}/login/{loginId}` | Update login credentials | Yes | Admin |
| DELETE | `/Users/{userId}/login/{loginId}` | Delete login credentials | Yes | Admin |

### Roles Endpoints

| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/Roles` | Get all roles | Yes | - |
| POST | `/Roles/initialize` | Initialize default roles | No | - |

---

## User Roles

### Client (Default)

**Permissions:**
- View films and genres
- Search and filter films
- Rate films (1-10)
- Edit own ratings
- Delete own ratings
- View and edit own profile
- View own ratings

**Restrictions:**
- Cannot create/edit/delete films
- Cannot manage genres
- Cannot manage other users
- Cannot delete other users' ratings

### Admin

**Permissions:**
- All Client permissions
- Create, edit, delete films
- Upload film images
- Create, edit, delete genres
- Create, edit, delete users
- Manage user login credentials
- Update/delete any rating
- View "My Films" (authored films)

---

## Screenshots & UI

### Layout Structure

The application uses a responsive layout with:

- **Sidebar Navigation** - Collapsible menu with role-based links
- **Top Bar** - User greeting, navigation links, logout
- **Main Content** - Dynamic page content
- **Footer** - Quick links and copyright

### Pages Overview

| Page | Route | Description |
|------|-------|-------------|
| Home | `/` | Landing page with welcome message |
| Films | `/films` | Film catalog with search/filter/sort |
| Film Details | `/films/{id}` | Single film view with rating |
| My Films | `/films/my-films` | Admin's authored films |
| Genres | `/genres` | Genre listing |
| Ratings | `/ratings` | Manage own ratings (edit/delete) |
| Users | `/users` | User management (admin) |
| Profile | `/profile` | User profile view |
| Login | `/login` | Login form |
| Register | `/register` | Registration form |

### Styling

- **CSS Framework:** Bootstrap 5.x
- **Icons:** Bootstrap Icons
- **Responsive:** Mobile-friendly design
- **Theme:** Clean, modern interface with sidebar navigation

---

## Development

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run with watch (auto-reload)
dotnet watch run
```

### Code Style

The project follows .NET conventions:
- PascalCase for classes, methods, properties
- camelCase for local variables, parameters
- Async/await for I/O operations
- Repository pattern for data access

### Adding New Features

1. **Add Model** - Create entity in `CinemaAPI/Models/`
2. **Update DbContext** - Add DbSet to `CinemaDbContext`
3. **Create Migration** - `dotnet ef migrations add MigrationName`
4. **Update Database** - `dotnet ef database update`
5. **Create Controller** - Add CRUD endpoints
6. **Add Frontend Service** - Create service in `CinemaBlazor/Services/`
7. **Create Pages** - Add Razor components

---

## Testing

### Manual Testing Checklist

**Authentication:**
- [ ] User registration
- [ ] User login
- [ ] JWT token expiration
- [ ] Profile update
- [ ] Logout

**Films:**
- [ ] Browse films
- [ ] Search films
- [ ] Filter by genre
- [ ] Sort by different fields
- [ ] Create film (admin)
- [ ] Upload image
- [ ] Edit film (admin)
- [ ] Delete film (admin)

**Ratings:**
- [ ] Submit rating
- [ ] Update own rating
- [ ] Delete own rating
- [ ] View average rating
- [ ] Cannot update/delete other user's rating (client)
- [ ] Can update/delete any rating (admin)

**Admin:**
- [ ] User management
- [ ] Genre management
- [ ] Rating management

---

## Troubleshooting

### Common Issues

**1. Database Connection Error**

```
Error: Connection refused
```

**Solution:** Ensure PostgreSQL is running and connection string is correct.

```bash
# Check PostgreSQL status
pg_ctl status

# Restart PostgreSQL
pg_ctl restart
```

**2. JWT Token Invalid**

```
Error: Invalid token
```

**Solution:** Check JWT key in appsettings.json matches between frontend and backend.

**3. CORS Error**

```
Error: Access to fetch has been blocked by CORS policy
```

**Solution:** Add your frontend URL to `CorsOrigins` in appsettings.json.

**4. Image Upload Fails**

```
Error: File too large or invalid format
```

**Solution:** Check `ImageSettings` in appsettings.json for size limits and allowed extensions.

**5. Port Already in Use**

```
Error: Failed to bind to address https://localhost:7156
```

**Solution:** Change port in `launchSettings.json` or stop the conflicting process.

```bash
# Find process on port 7156 (Windows)
netstat -ano | findstr :7156

# Kill process
taskkill /PID <PID> /F
```

**6. User Cannot Update Own Rating**

```
Error: 403 Forbidden
```

**Solution:** Ensure the user is the owner of the rating. Ownership is verified by `AuthorId` in the database.

---

## Sample Data

The `insert_sql.sql` file includes:

- **2 Roles:** admin, client
- **5 Genres:** Sci-Fi, Drama, Action, Comedy, Horror
- **6 Users:** 1 admin + 5 clients
- **25 Films:** Russian cinema across all genres
- **50 Ratings:** Multiple ratings per film
- **5 Logins:** One per non-admin user

### Default Admin Credentials

After running the seed script:

| Login | Password | Role |
|-------|----------|------|
| admin_admin | admin123 | Admin |

---

## License

This project is for educational purposes.

---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## Support

For issues and questions:
- Open an issue on GitHub
- Check the troubleshooting section
- Review API Swagger documentation at `/swagger`

---

## Future Enhancements

- [ ] Film reviews/comments
- [ ] Watchlist functionality
- [ ] Advanced search with multiple criteria
- [ ] Film trailers
- [ ] Actor/director information
- [ ] Email notifications
- [ ] Password reset functionality
- [ ] Two-factor authentication
- [ ] Film recommendations
- [ ] User activity history

---

*Last updated: March 2026*
