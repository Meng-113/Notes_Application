# Notes Application

A full-stack notes app with a Vue 3 frontend and an ASP.NET Core Web API backend. Users can register, log in with JWT authentication, and manage personal notes stored in SQL Server.

## Stack

- Frontend: Vue 3, TypeScript, Vite, Pinia, Tailwind CSS 4, Axios
- Backend: ASP.NET Core 8 Web API, Dapper, JWT Bearer authentication
- Database: SQL Server

## Features

- User registration and login
- JWT-based authenticated API access
- Create, read, update, and delete notes
- Search notes by title/content
- Filter notes by all, with content, or empty
- Responsive notes layout for desktop and mobile

## Project Structure

```text
Notes_Application/
|-- Notes_Frontend/         # Vue + Vite client
|-- Note_Backend/           # ASP.NET Core backend
|   `-- Note_Backend/       # Main Web API project
`-- README.md
```

## How It Works

- The frontend stores the JWT token and username in `localStorage`.
- The backend exposes auth endpoints under `/api/Auth` and protected note endpoints under `/api/Notes`.
- The backend is configured to allow the Vite dev server on `http://localhost:5173`.
- The frontend defaults to `/api` as its base URL, so for local split development you should set `VITE_API_BASE_URL` explicitly.

## Prerequisites

- Node.js `^20.19.0` or `>=22.12.0`
- npm
- .NET 8 SDK
- SQL Server or SQL Server Express/LocalDB
- Windows authentication access to SQL Server if you use the default connection string

## Backend Setup

### 1. Configure app settings

The backend reads configuration from:

- `Note_Backend/Note_Backend/appsettings.json`
- `Note_Backend/Note_Backend/appsettings.Development.json`

Default values already exist for:

- `ConnectionStrings:DefaultConnection`
- `JwtConfig:Issuer`
- `JwtConfig:Audience`
- `JwtConfig:Key`
- `JwtConfig:TokenValidityMins`

The current default connection string is:

```json
"Server=localhost;Database=NoteDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

If your SQL Server instance differs, update that value before starting the API.

### 2. Create the database

This project does not include EF migrations. The schema is expected to exist already. You can create it with SQL similar to:

```sql
CREATE DATABASE NoteDb;
GO

USE NoteDb;
GO

CREATE TABLE UserAccounts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL
);
GO

CREATE TABLE Notes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL CONSTRAINT DF_Notes_CreatedAt DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL CONSTRAINT DF_Notes_UpdatedAt DEFAULT GETDATE(),
    CONSTRAINT FK_Notes_UserAccounts FOREIGN KEY (UserId) REFERENCES UserAccounts(Id)
);
GO

CREATE INDEX IX_Notes_UserId_UpdatedAt ON Notes(UserId, UpdatedAt DESC, Id DESC);
GO
```

## Frontend Setup

### 1. Install dependencies

```powershell
cd Notes_Frontend
npm install
```

### 2. Configure the API base URL

Create `Notes_Frontend/.env.local`:

```env
VITE_API_BASE_URL=http://localhost:5028/api
```

Use `https://localhost:7009/api` instead if you prefer the HTTPS profile.

Without this variable, the frontend falls back to `/api`, which is only appropriate if the frontend and backend are served from the same origin or behind a reverse proxy.

## Run Locally

### Start the backend

```powershell
cd Note_Backend\Note_Backend
dotnet restore
dotnet run
```

Development launch settings expose:

- `http://localhost:5028`
- `https://localhost:7009`

Swagger is available in development at:

- `http://localhost:5028/swagger`
- `https://localhost:7009/swagger`

### Start the frontend

In a second terminal:

```powershell
cd Notes_Frontend
npm run dev
```

Vite runs by default at:

- `http://localhost:5173`

## API Overview

### Auth

- `POST /api/Auth/register`
- `POST /api/Auth/login`

Request body:

```json
{
  "Username": "alice",
  "Password": "secret123"
}
```

Response body:

```json
{
  "Username": "alice",
  "Token": "<jwt>",
  "ExpiresAt": "2026-03-29T12:34:56.0000000Z"
}
```

### Notes

All note endpoints require:

```http
Authorization: Bearer <jwt>
```

- `GET /api/Notes`
- `GET /api/Notes/{id}`
- `POST /api/Notes`
- `PUT /api/Notes/{id}`
- `DELETE /api/Notes/{id}`

Create/update payload:

```json
{
  "Title": "Meeting notes",
  "Content": "Discuss release scope and deadline."
}
```

## Useful Commands

### Frontend

```powershell
cd Notes_Frontend
npm run dev
npm run build
npm run type-check
```

### Backend

```powershell
cd Note_Backend\Note_Backend
dotnet run
dotnet build
```

## Notes For Production

- Passwords are currently hashed with SHA-256. For production, use a strong adaptive password hasher such as ASP.NET Core Identity's password hasher, bcrypt, or Argon2.
- JWT settings are stored in `appsettings.json`. Move secrets to environment variables or a secure secret store before deploying.
- The frontend stores the token in `localStorage`, which is acceptable for simple demos but should be reviewed for stricter security requirements.
