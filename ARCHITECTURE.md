# 📐 Technical Architecture Specification

## Overview

**WebAnalyticsAggregator** is designed as an enterprise-grade, distributed message-driven analytics aggregation pipeline. The architecture decouples data collection, asynchronous processing/reconciliation, relational storage, and API delivery.

---

## 🏛️ Architectural Design Patterns

### 1. Clean Architecture (Onion Architecture)
The application adheres strictly to dependency inversion and onion architecture boundaries:
- **Domain Layer (`Domain`)**: Core entities, base domain primitives, domain exceptions, independent of any frameworks.
- **Application Layer (`Application`)**: Use case implementations (`ReportService`, `UserService`), business logic interfaces, and application DTOs.
- **Infrastructure Layer (`Infrastructure`)**: Persistence (`AnalyticsDbContext`), SQL Server Repositories (`AnalyticsRepository`, `UserRepository`), Security services (`JwtProvider`, `PasswordHasher`).
- **Presentation / Ingestion Layer (`WebAnalyticsAggregator`, `Producer`, `Worker`)**: API Endpoints, background message consumers, data readers, and queue publishers.

### 2. Event-Driven Architecture (Producer-Consumer)
- Data ingestion is completely decoupled from database writes.
- The **Producer Service** reads raw streaming metrics from simulated Google Analytics (GA) and PageSpeed Insights (PSI) sources.
- Messages are published asynchronously to **RabbitMQ** queue `analytics.raw.q`.
- The **Worker (Consumer Service)** listens to RabbitMQ, parses incoming JSON payloads, correlates incoming metrics by URL & Date, and persists aggregated analytics directly into SQL Server.

---

## 🔄 Correlation Algorithm Engine

When analytics records arrive from distinct telemetry sources, they contain separate facets of web telemetry:
- **Google Analytics Payload**: `Page`, `Date`, `Pageviews`, `UniqueVisitors`, `BounceRate`
- **PageSpeed Insights Payload**: `Page`, `Date`, `FcpMs`, `LcpMs`, `Cls`, `PerformanceScore`

### Correlation Sequence:
1. Message arrives in `Worker`.
2. Worker checks if an existing entry exists in `CombinedRecords` table matching key `(Page, Date)`.
3. If entry exists, existing record is updated with new metrics (e.g. populating PSI fields when GA fields already exist).
4. If entry does not exist, a new `CombinedRecord` is inserted.
5. Daily summary aggregates (`DailyStats`) are recalculated for high-performance API query serving.

---

## 🗄️ Database Schema & Entity Design

```
+-------------------------------------------------------------+
|                          Users                              |
+-------------------------------------------------------------+
| Id (Guid, PK)                                               |
| Name (NVARCHAR(100))                                        |
| Email (NVARCHAR(200), UNIQUE INDEX)                         |
| PasswordHash (NVARCHAR(MAX))                                |
| CreatedAt (DATETIME2)                                       |
+-------------------------------------------------------------+

+-------------------------------------------------------------+
|                     CombinedRecords                         |
+-------------------------------------------------------------+
| Id (Guid, PK)                                               |
| Page (NVARCHAR(500), INDEX)                                 |
| Date (DATE, INDEX)                                          |
| Pageviews (INT)                                             |
| UniqueVisitors (INT)                                        |
| BounceRate (FLOAT)                                          |
| FcpMs (FLOAT)                                               |
| LcpMs (FLOAT)                                               |
| Cls (FLOAT)                                                 |
| PerformanceScore (FLOAT)                                    |
| CreatedAt (DATETIME2)                                       |
+-------------------------------------------------------------+
```

---

## 🔒 Security Architecture

1. **Password Hashing**: Uses PBKDF2 / BCrypt hashing algorithms with cryptographically secure random salts. Plaintext passwords are never stored or logged.
2. **JWT Authorization**: 
   - SHA-256 HMAC signed tokens containing User Claims (`Name`, `Email`, `UserId`).
   - Standard ASP.NET Core Bearer Authentication middleware validates issuer, audience, signature key, and token expiration.
