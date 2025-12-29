# Mizni URL

Mizni URL is a tool to shorten a long link and create a short URL easy to share on sites, chat and emails. Track short URL traffic and manage your links.

## Architecture

Microservices-ready architecture with Clean Architecture principles:

```
mizni-url/
├── URLService/              # URL shortening service
│   ├── Domain/              # Core entities and interfaces
│   ├── Application/         # CQRS handlers, business logic
│   ├── Infrastructure/      # MongoDB, Redis, Hangfire
│   ├── WebAPI/             # REST API endpoints
│   └── StressTest/         # K6 performance testing
└── Contracts/              # Shared contracts between services
```

**Clean Architecture Layers:**
- **Domain**: Entities (UrlEntry, Sequence), value objects, interfaces
- **Application**: CQRS with WolverineFx, services, background workers
- **Infrastructure**: Data access, caching, external integrations
- **WebAPI**: Minimal APIs, middleware, DI configuration

## What Makes This Fast

**Blazing Fast Redirect Response**
- **HybridCache** (L1 + L2): In-memory cache (2-5min) + Redis distributed cache (10-30min)
- **Hot Key Detection**: Lua script tracks access patterns, auto-promotes viral URLs to longer TTL
- **Bloom Filter**: Prevents cache penetration for non-existent URLs
- **Negative Caching**: Caches 404s locally for 60s to avoid repeated DB queries
- **Direct MongoDB Collection Access**: Bypasses EF Core tracking for read-only queries
- **Background Task Queue**: Hot key detection runs async, doesn't block response

**Why MongoDB + EF Core?**
- **Best of Both Worlds**: EF Core's familiar API + MongoDB's performance and flexibility
- **Direct Collection Access**: Use `GetCollection<T>()` for high-performance queries bypassing change tracking
- **Atomic Operations**: `FindOneAndUpdate` with optimistic concurrency for sequence generation
- **Replica Sets**: High availability with automatic failover
- **No Schema Migrations**: Document model adapts to changes without downtime

**Redis Lua Scripts**
- **Atomic Hot Key Detection**: Single round-trip increments counter, updates sorted set, checks threshold
- **Race Condition Free**: All operations execute atomically on Redis server
- **Efficient**: No network overhead for multiple commands

**Smart Caching Strategy**
- Normal URLs: 10min Redis / 2min local
- Hot URLs (50+ hits/min): 30min Redis / 5min local
- Negative cache: 60s local only
- Auto-prewarm: Background worker refreshes top 100 hot keys before expiry

**Performance Optimizations**
- Hashids for non-sequential short codes (prevents enumeration)
- MongoDB sequence with atomic increment
- Channel-based background task queue
- Rate limiting to prevent abuse
- K6 stress testing (steady, burst, spike, soak scenarios)

## Tech Stack

- .NET 10, ASP.NET Core Minimal APIs
- MongoDB 7.0 with replica sets + EF Core provider
- Redis (distributed cache + Lua scripts)
- Hangfire with MongoDB storage
- WolverineFx (CQRS messaging)
- HybridCache
- Bloom Filter (cache penetration protection)
- Serilog (structured logging)
- MiniProfiler (performance profiling)
- Scalar (API documentation)
- K6 (load testing)

## TODO

- [ ] **UserService** - Authentication and user-owned URLs
- [ ] **Gateway** - YARP reverse proxy for service routing
- [ ] **AnalyticService** - Advanced traffic analytics and link management dashboard