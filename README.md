# SlideSwiperApi

A production-ready **.NET 10 Web API** for managing eCommerce homepage slides, built with **Domain-Driven Design (DDD)**, **CQRS**, **MediatR**, **Redis caching**, and deployed on **Railway**.

---

##  Live Deployment

| Resource | URL |
|---|---|
| **API Base URL** | `https://slideswiperapi-production.up.railway.app` |
| **Slides Endpoint** | `https://slideswiperapi-production.up.railway.app/api/slides` |

---

##  Architecture — Domain-Driven Design (DDD)

The solution is split into 4 distinct layers, each with a single responsibility:

```
SlideSwiperApi/
├── src/
│   ├── Domain/          → Core business entities. No dependencies on anything.
│   ├── Application/     → CQRS commands, queries, handlers, DTOs, validators.
│   ├── Infrastructure/  → EF Core, PostgreSQL, Redis, repository implementations.
│   └── Api/             → Controllers, Program.cs, middleware, rate limiting.
│
└── tests/
    └── Tests/           → Unit tests for handlers and validators.
```

### Why DDD?
- **Domain** layer has zero external dependencies — business rules are pure C#
- **Application** layer orchestrates use cases via CQRS without knowing how data is stored
- **Infrastructure** layer handles all I/O (database, cache) behind interfaces
- **Api** layer is thin — it only routes HTTP requests to MediatR

---

## ⚙️ Tech Stack

| Technology | Purpose |
|---|---|
| **.NET 10** | Web API framework |
| **EF Core + Npgsql** | ORM + PostgreSQL provider |
| **PostgreSQL** | Primary database (hosted on Railway) |
| **Redis** | Response caching (hosted on Railway) |
| **MediatR** | CQRS mediator pattern |
| **FluentValidation** | Input validation pipeline |
| **Swagger / Swashbuckle** | API documentation |
| **Docker** | Containerization for Railway deployment |
| **xUnit + Moq + FluentAssertions** | Unit testing |

---

##  CQRS Pattern

Every operation goes through MediatR as either a **Command** (write) or a **Query** (read):

```
HTTP Request
    ↓
SlidesController
    ↓
IMediator.Send(Command or Query)
    ↓
Handler (in Application layer)
    ↓
ISlideRepository (interface) → Infrastructure → PostgreSQL
ICacheService (interface)    → Infrastructure → Redis
```

### Commands (write operations)
| Command | Description |
|---|---|
| `CreateSlideCommand` | Creates a new slide, invalidates Redis cache |
| `UpdateSlideCommand` | Updates existing slide, invalidates Redis cache |
| `DeleteSlideCommand` | Deletes slide, invalidates Redis cache |

### Queries (read operations)
| Query | Description |
|---|---|
| `GetAllSlidesQuery` | Returns all active slides. Checks Redis first, falls back to DB |
| `GetSlideByIdQuery` | Returns a single slide by ID. Checks Redis first, falls back to DB |

---

## 🗃️ Redis Caching Strategy

Redis is used to avoid repeated database reads for frequently accessed data:

- `GET /api/slides` → cached under key `slides:all` for **10 minutes**
- `GET /api/slides/{id}` → cached under key `slides:{id}` for **10 minutes**
- Any **create**, **update**, or **delete** → immediately **invalidates** the relevant cache keys

This ensures the API serves cached responses in milliseconds while keeping data consistent after writes.

---

## ✅ Validation

All create and update operations pass through a **FluentValidation pipeline behavior** registered in MediatR. This means validation runs automatically before any handler is executed — no manual validation calls needed in the controller.

### Slide entity required fields:
| Field | Rule |
|---|---|
| `ImageUrl` | Required, max 500 characters |
| `Title1` | Required, max 200 characters |
| `Title2` | Required, max 200 characters |
| `Title3Part1` | Required, max 200 characters |
| `Title3Part2` | Required, max 200 characters |
| `Title3Part3` | Optional, max 200 characters |
| `Title4` | Required, max 200 characters |
| `Order` | Required, must be ≥ 0 |

---

##  Rate Limiting

The API uses .NET's built-in **Fixed Window Rate Limiter**:
- **30 requests per minute** per client
- Exceeding the limit returns **HTTP 429 Too Many Requests**
- Configured on the `SlidesController` via `[EnableRateLimiting("fixed")]`

---

##  API Endpoints

Base URL: `https://slideswiperapi-production.up.railway.app`

| Method | Endpoint | Description | Cache |
|---|---|---|---|
| `GET` | `/api/slides` | Get all active slides (ordered) |  Redis |
| `GET` | `/api/slides/{id}` | Get slide by ID |  Redis |
| `POST` | `/api/slides` | Create new slide |  Invalidates cache |
| `PUT` | `/api/slides/{id}` | Update existing slide |  Invalidates cache |
| `DELETE` | `/api/slides/{id}` | Delete a slide |  Invalidates cache |

### Example Request — Create Slide
```http
POST /api/slides
Content-Type: application/json

{
  "imageUrl": "https://example.com/image.jpg",
  "title1": "WELCOME TO OUR STORE",
  "title2": "New & Casual Collection",
  "title3Part1": "Sale up to",
  "title3Part2": "30% OFF",
  "title3Part3": "Trendy",
  "title4": "Free shipping on all your orders.",
  "order": 1
}
```

### Example Response
```json
{
  "id": "9a9aa496-3e61-4f8c-81d0-1c86015847ce",
  "imageUrl": "https://example.com/image.jpg",
  "title1": "WELCOME TO OUR STORE",
  "title2": "New & Casual Collection",
  "title3Part1": "Sale up to",
  "title3Part2": "30% OFF",
  "title3Part3": "Trendy",
  "title4": "Free shipping on all your orders.",
  "order": 1,
  "isActive": true,
  "createdAt": "2026-04-13T23:36:22.685095Z",
  "updatedAt": null
}
```

---

## 🧪 Tests

Complete unit test suite with **50+ comprehensive tests** organized by feature. Uses **xUnit**, **Moq**, **FluentAssertions**, and **test fixtures**.

### Test Structure
```
tests/Tests/
├── Unit/
│   ├── Validators/
│   │   ├── CreateSlideValidatorTests.cs        (13 tests)
│   │   └── UpdateSlideValidatorTests.cs        (3 tests)
│   └── Handlers/
│       ├── Commands/
│       │   └── CreateSlideHandlerTests.cs      (9 tests)
│       └── Queries/
│           ├── GetAllSlidesHandlerTests.cs     (11 tests)
│           └── GetSlideByIdHandlerTests.cs     (11 tests)
└── TestFixtures/
    ├── SlideCommandBuilder.cs       (Fluent builder pattern)
    └── SlideDtoBuilder.cs          (Fluent builder pattern)
```

### Test Coverage

#### **CreateSlideValidatorTests** (13 tests)
- ✅ Valid commands pass validation
- ✅ Rejects empty `ImageUrl`, `Title1`, `Title2`, `Title3Part1`, `Title3Part2`, `Title4`
- ✅ Allows null `Title3Part3` (optional field)
- ✅ Rejects negative `Order`, accepts zero
- ✅ Enforces max length constraints (500 for URLs, 200 for titles)
- ✅ Multiple validation errors reported together

#### **UpdateSlideValidatorTests** (3 tests)
- ✅ Valid commands pass validation
- ✅ Rejects empty `Id` (GUID must not be Guid.Empty)
- ✅ Accepts valid IDs

#### **CreateSlideHandlerTests** (9 tests)
- ✅ Calls repository `AddAsync` with correct slide data
- ✅ Calls `SaveChangesAsync` exactly once
- ✅ Invalidates cache key `slides:all` after creation
- ✅ Returns `SlideDto` with all properties correctly mapped
- ✅ Returns non-empty GUID for `Id`
- ✅ Sets `IsActive = true`
- ✅ Sets `CreatedAt` timestamp within request time range
- ✅ Propagates repository exceptions
- ✅ Passes `CancellationToken` to repository methods

#### **GetAllSlidesHandlerTests** (11 tests)
- ✅ **Cache hit**: Returns cached data without calling repository
l- ✅ **Cache miss**: Calls repository and caches the result
- ✅ **Cache miss**: Uses correct cache key `slides:all`
- ✅ Returns empty list when no slides exist
- ✅ Handles cache errors gracefully
- ✅ Maintains slide order from repository
- ✅ Maps `Slide` to `SlideDto` correctly
- ✅ Handles multiple slides efficiently
- ✅ Sets cache expiry (10 minutes)
- ✅ Passes `CancellationToken` correctly

#### **GetSlideByIdHandlerTests** (11 tests)
- ✅ **Cache hit**: Returns cached slide without calling repository
- ✅ **Cache miss**: Calls repository and caches the result
- ✅ **Cache miss**: Uses correct cache key format `slides:{id}`
- ✅ Returns `null` when slide not found
- ✅ Does NOT cache when slide not found
- ✅ Maps `Slide` to `SlideDto` correctly
- ✅ Handles different slide IDs properly
- ✅ Passes `CancellationToken` correctly
- ✅ Returns correct slide data on cache miss

### Running Tests

**Run all tests:**
```bash
dotnet test
```

**Run with coverage:**
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Run specific test class:**
```bash
dotnet test --filter "ClassName=Tests.Unit.Validators.CreateSlideValidatorTests"
```

**Run and show detailed output:**
```bash
dotnet test -v detailed
```

### Test Patterns Used

#### **Builder Pattern** (Test Fixtures)
```csharp
var command = new CreateSlideCommandBuilder()
    .WithTitle1("Custom Title")
    .WithOrder(5)
    .Build();
```

#### **Moq Mocking**
```csharp
_mockRepository.Setup(r => r.AddAsync(It.IsAny<Slide>(), ...))
    .Returns(Task.CompletedTask);
```

#### **FluentAssertions**
```csharp
result.Should().NotBeNull();
result.ImageUrl.Should().Be(command.ImageUrl);
result.Errors.Should().ContainSingle(e => e.PropertyName == "Title1");
```

### Dependencies
- **xUnit** — Testing framework
- **Moq 4.20.72** — Mocking library
- **FluentAssertions 8.9.0** — Assertion library
- **FluentValidation 11.9.2** — Validator testing
- **coverlet.collector** — Code coverage

---

##  Local Development

### Prerequisites
- .NET 10 SDK
- PostgreSQL running locally (or use Railway public URL)
- Redis running locally (or use Railway public URL)

###  Setup

```bash
git clone https://github.com/RashedKlo/SlideSwiperApi.git
cd SlideSwiperApi
```

Update `src/Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SlideSwiperDb;Username=postgres;Password=yourpassword",
    "Redis": "localhost:6379"
  }
}
```

Run migrations:
```bash
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Api
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

Start the API:
```bash
dotnet run --project src/Api
```

### Running Tests Locally

```bash
# Run all tests
dotnet test

# Run with coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "ClassName=Tests.Unit.Validators.CreateSlideValidatorTests"

# Run with verbose output
dotnet test -v detailed
```

### Docker Deployment

```bash
# Build and run with Docker Compose (includes PostgreSQL, Redis, and optional tests)
docker-compose up

# Build specific service
docker-compose up api

# Run tests in Docker
docker-compose run tests
```

---

**Rashed Klo** — Full Stack Developer  
GitHub: [github.com/RashedKlo](https://github.com/RashedKlo)  
Email: rashed.klo.dev@gmail.com