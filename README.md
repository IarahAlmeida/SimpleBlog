# SimpleBlog

A clean architecture blog application built with .NET 9, demonstrating DDD, CQRS, and event-driven patterns.

## Features
- Post creation and management
- Commenting system
- Domain events
- Clean architecture implementation

### Technologies
- .NET 9
- ASP.NET Core
- Entity Framework Core
- MediatR
- XUnit
- NSubstitute
- Docker
- PostgreSQL

## Getting Started

### Prerequisites
- Docker Desktop
- .NET 9 SDK (for development)

### Setup

1. Clone the repository

2. Create `.env` file in root directory:
```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DB=simpleblog
POSTGRES_SCHEMA=public
```

3. Start the application:
```bash
docker compose --profile api up -d
```

The API will be available at `http://localhost:8080` (or configured port).

To stop the application:
```bash
docker compose --profile api down
```

## API Endpoints
- `POST /api/posts` - Create new post
- `GET /api/posts` - Get all posts
- `GET /api/posts/{id}` - Get post by ID
- `POST /api/posts/{id}/comments` - Add comment to post

## Testing
```bash
dotnet test
```