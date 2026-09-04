# ⚡ Scalable Real-Time Messaging Engine

High-performance, event-driven real-time chat backend built with **ASP.NET Core**, **SignalR**, **RabbitMQ (MassTransit)**, **Redis**, and **PostgreSQL**. 

Designed with Clean Architecture principles to demonstrate distributed systems concepts: decoupled message delivery, horizontal socket scaling, and secure token-based authentication.

---

## 🏗 Architecture & Engineering Highlights

- **Horizontal SignalR Scaling:** Integrated **Redis Backplane** via `Microsoft.AspNetCore.SignalR.StackExchangeRedis`, allowing WebSockets state and message broadcasting to synchronize across multiple server instances.
- **Event-Driven Messaging (Fan-Out Pattern):** Implemented asynchronous message dispatch using **MassTransit** over **RabbitMQ** (`MessageFanOutConsumer`), offloading message delivery bottlenecks from the API layer.
- **Distributed Caching:** Leveraged Redis (`StackExchange.Redis` & `IDistributedCache`) for session management and low-latency data access.
- **Clean / Layered Architecture:** Clear separation of concerns into Domain entities, Application business logic & contracts, and Infrastructure/Persistence adapters.
- **Secure Authentication:** JWT bearer token lifecycle, custom `IPasswordHasher<User>` for secure credential hashing, and fine-grained authorization middleware.
- **Persistence Layer:** PostgreSQL managed via Entity Framework Core with code-first migrations and optimized repository abstractions.

---

## 🛠 Tech Stack

- **Framework:** .NET 8 / ASP.NET Core Web API
- **Real-Time Engine:** SignalR + Redis Backplane
- **Message Broker:** RabbitMQ via MassTransit
- **Caching & In-Memory:** Redis (`StackExchange.Redis`)
- **Database:** PostgreSQL (Npgsql Entity Framework Core)
- **Security:** JWT (JSON Web Tokens), ASP.NET Core Identity Password Hasher
- **Frontend Integration:** React (Vite / localhost:5173 with CORS Credentials enabled)

---

## 🚀 System Architecture Overview

[ React Client ]
│
▼ (HTTP / WebSockets)
[ ASP.NET Core Web API ]
├── JWT Auth Middleware
├── SignalR Hub (/chatHub) ──▶ [ Redis Backplane (Sync instances) ]
└── Application Core
├── EF Core ────────────▶ [ PostgreSQL Database ]
├── Redis Cache ────────▶ [ Redis Cache Store ]
└── MassTransit ────────▶ [ RabbitMQ Exchange ]
│
▼ (Fan-Out Consumer)
[ Message Dispatch Worker ]
---

## ⚙️ Local Development Setup

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose (or local instances of PostgreSQL, Redis, RabbitMQ)

### 1. Environment Configuration
Create a `.env` file in the project root:
```env
DB_NAME=chat_db
DB_USER=postgres
DB_PASSWORD=your_password
Verify appsettings.json connection strings:

JSON
"ConnectionStrings": {
  "Redis": "localhost:6379"
}
2. Run Dependencies (Docker)
Bash
docker run -d --name chat-postgres -p 5432:5432 -e POSTGRES_PASSWORD=your_password -e POSTGRES_DB=chat_db postgres
docker run -d --name chat-redis -p 6379:6379 redis
docker run -d --name chat-rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
3. Run Backend
Bash
dotnet restore
dotnet ef database update
dotnet run

---

### Топ-4 підступні питання, які тобі поставлять на технічній співбесіді

Будь готовий відповісти на них своїми словами:

#### 1. «Навіщо ти додав Redis до SignalR (`AddStackExchangeRedis`)?»
* **Відповідь:** «Якщо запустити бекенд у двох або трьох інстансах за Load Balancer, користувач А підключиться до Сервера 1, а користувач Б — до Сервера 2. За звичайних умов Сервер 1 нічого не знає про вебсокети Сервера 2. **Redis Backplane виступає шиною подій**: коли Сервер 1 шле повідомлення в хаб, воно публікується в Redis Pub/Sub, і Сервер 2 теж доставляє його своєму підключеному клієнту».

#### 2. «Що робить `MessageFanOutConsumer` у RabbitMQ?»
* **Відповідь:** «Він реалізує патерн Fan-Out. Коли надходить повідомлення, ми не блокуємо HTTP-запит записом у базу для кожного учасника групового чату. Ми публікуємо подію в RabbitMQ, а консьюмер асинхронно розсилає копії повідомлення всім підписникам/учасникам кімнати».

#### 3. «Чому в тебе в коді двічі викликається `builder.Services.AddCors`?»
* **Зверни увагу на баг у своєму `Program.cs`:** ти зареєстрував CORS двічі — спочатку з політикою за замовчуванням, а потім із назвою `"AllowFrontend"`. 
* **Порада:** прибери дублікат, залиш один:
  ```csharp
  builder.Services.AddCors(options =>
  {
      options.AddPolicy("AllowFrontend", policy =>
      {
          policy.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // обов'язково для SignalR з токенами
      });
  });
