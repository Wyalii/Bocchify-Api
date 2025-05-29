# 📡 Anime & Manga Backend API

This is the backend API for the Anime & Manga app. It handles user authentication, saving favourites, and serves secure endpoints to be used by the frontend app [**Bocchify**](https://github.com/Wyalii/Bocchify-Api.git). Built using **.NET 9 Web API** and **PostgreSQL**.

---

## 🚀 Features

- 🔐 User registration and login (JWT-based auth)
- 📥 Add/remove favourites (Anime or Manga)
- 📧 Password reset via email
- 🧠 Secure token validation using JWT
- 🌍 CORS enabled for frontend integration
- 📦 Environment variables via `.env`

---

## 🛠 Tech Stack

- [.NET 9 Web API](https://dotnet.microsoft.com/)
- [PostgreSQL](https://www.postgresql.org/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://jwt.io/)
- [DotEnv.NET](https://github.com/bolorundurowb/dotenv.net)
- [Swagger / Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

---

## 🗂️ Project Structure (Simplified)

```text
📦 Bocchify-Api/
├── Controllers/
│   ├── AuthController.cs          # Handles login, register, verifying of account
│   ├── FavouriteController.cs     # Handles adding/removing/getting favourites
│   ├── PasswordController.cs      # Handles password reset (forgot password feature)
│   └── UserController.cs          # Handles updating user profile (name, profile image)
│
├── Models/
│   ├── User.cs                    # User entity model
│   └── Favourite.cs              # Favourite entity model
│
├── Repositories/
│   ├── UsersRepository.cs         # Data logic for users
│   └── FavouritesRepository.cs    # Data logic for favourites
│
├── Services/
│   ├── TokenService.cs            # JWT generation and validation
│   ├── PasswordService.cs         # Password hashing/verification
│   └── MailService.cs             # Sends password reset emails
│
├── appsettings.json              # App configuration (local)
├── Program.cs                    # Main app entry point (configures middleware, DI, auth)
├── .env                          # Environment variables (not committed)
└── Bocchify_Server.csproj        # Project file


