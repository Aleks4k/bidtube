# 🎬 BidTube API

> A RESTful API backend for the BidTube auction platform — built with **ASP.NET Core 9** following **Clean Architecture** principles.

---

## 📖 About

BidTube is an online auction platform where users can create auctions, place bids, receive real-time notifications, and authenticate via standard login or Google OAuth. The backend is built with Clean Architecture, CQRS via MediatR, and real-time communication via SignalR.

---

## 🏗️ Architecture

```
bidtube/
├── bidtube.Api/             # Controllers, SignalR hub, filters, startup
├── bidtube.Application/     # CQRS commands/queries, DTOs, validators, interfaces
├── bidtube.Domain/          # EF Core entities, DbContext, migrations
└── bidtube.Infrastructure/  # Repositories, JWT, Cloudinary, Google Auth, background services
```

| Layer | Responsibility |
|---|---|
| **Api** | HTTP request handling, SignalR hub, FluentValidation wiring |
| **Application** | Use cases (MediatR), DTOs, validators, service contracts |
| **Domain** | Entities (`User`, `Auction`, `Bid`, `Notification`, …), `AppDbContext` |
| **Infrastructure** | MySQL via EF Core, JWT service, Cloudinary, Google OAuth, `AuctionEndService` background job |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- MySQL server
- A [Cloudinary](https://cloudinary.com/) account
- A Google OAuth client ID

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Aleks4k/bidtube.git
   cd bidtube
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Create a `.env` file** in the root of `bidtube.Api/` with the following variables:
   ```env
   MYSQL_CONNECTION=server=localhost;database=bidtube;user=root;password=yourpassword

   CLOUDINARY_URL=cloudinary://api_key:api_secret@cloud_name

   GoogleSettings_ClientId=your_google_client_id

   JWT_AccessTokenKey=your_access_token_secret
   JWT_RefreshTokenKey=your_refresh_token_secret
   JWT_Issuer=bidtube
   JWT_Audience=bidtube
   JWT_AccessTokenTTL=900000
   JWT_RefreshTokenTTL=7
   ```
   > `JWT_AccessTokenTTL` is in milliseconds. `JWT_RefreshTokenTTL` is in days.

4. **Apply database migrations**
   ```bash
   dotnet ef database update --project bidtube.Domain --startup-project bidtube.Api
   ```

5. **Run the API**
   ```bash
   dotnet run --project bidtube.Api
   ```

Swagger UI is available at `https://localhost:{port}/swagger` when running in Development mode.

---

## 📡 API Endpoints

All routes are prefixed with `/api/[controller]`. Most endpoints require a JWT Bearer token. Exceptions are marked as 🔓 public.

### 👤 User — `/api/user`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/login` | 🔓 Public | Login with email & password |
| POST | `/register` | 🔓 Public | Register a new account |
| POST | `/login-google` | 🔓 Public | Login or initiate registration via Google OAuth |
| POST | `/refresh` | 🔓 Public | Refresh the access token using a refresh token |
| POST | `/change-password-google` | 🔒 JWT | Complete Google registration by setting a password |
| POST | `/getUserData` | 🔒 JWT | Get public profile data for a user |
| POST | `/getUserEditData` | 🔒 JWT | Get editable profile fields for the current user |
| POST | `/updateUserData` | 🔒 JWT | Update profile information |
| POST | `/updateUserPassword` | 🔒 JWT | Change password |

### 🏷️ Auction — `/api/auction`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/addAuction` | 🔒 JWT | Create a new auction (with images via Cloudinary) |
| POST | `/getAuctions` | 🔒 JWT | Get paginated, sorted, and filtered auctions |
| POST | `/getAuctionsWithCategories` | 🔒 JWT | Get auctions grouped with their categories |

### 💰 Bid — `/api/bid`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/putOffer` | 🔒 JWT | Place a bid on an auction |

### 🗂️ Category — `/api/category`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/getCategories` | 🔒 JWT | Get all available auction categories |

### 🔔 Notification — `/api/notification`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/getNotifications` | 🔒 JWT | Get paginated notifications for the current user |
| GET | `/unread/count` | 🔒 JWT | Get count of unread notifications |
| POST | `/markAsRead` | 🔒 JWT | Mark a specific notification as read |
| POST | `/markAllAsRead` | 🔒 JWT | Mark all notifications as read |

---

## ⚡ Real-Time (SignalR)

A SignalR hub is available at `/hub`. It requires JWT authentication passed as a query parameter:

```
wss://yourdomain.com/hub?access_token=<your_jwt>
```

The hub handles connected user tracking and pushes real-time bid/notification events to connected clients.

---

## 🔐 Authentication

The API uses **JWT Bearer authentication** with two separate tokens:

- **Access Token** — short-lived (configurable TTL in ms), signed with `JWT_AccessTokenKey`, used for all protected endpoints.
- **Refresh Token** — long-lived (configurable TTL in days), signed with `JWT_RefreshTokenKey`, used only to obtain a new access token via `POST /api/user/refresh`.

Both tokens are validated against Issuer, Audience, Lifetime, and Signature. Clock skew is set to zero.

Google OAuth login is supported via `Google.Apis.Auth` token verification on the backend.

---

## 🧱 Tech Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core 9 | Web framework |
| Entity Framework Core 9 + Pomelo | ORM with MySQL |
| MediatR | CQRS (commands & queries) |
| FluentValidation | Request validation |
| SignalR | Real-time communication |
| JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`) | Authentication |
| Cloudinary | Auction image storage |
| Google.Apis.Auth | Google OAuth verification |
| SixLabors.ImageSharp | Server-side image processing |
| dotenv.net | Environment variable loading from `.env` |
| Swagger / Swashbuckle | API documentation |

---

## 📄 License

This project is licensed under the terms found in [LICENSE.txt](./LICENSE.txt).

---

## 👤 Author

**Aleks4k** — [@Aleks4k](https://github.com/Aleks4k)
