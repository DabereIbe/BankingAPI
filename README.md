# Banking API

A secure, layered RESTful Banking API built with **ASP.NET Core 8**, **Entity Framework Core**, **SQL Server**, and **JWT Authentication**.

The project follows a **Clean Architecture** consisting of:

* Presentation Layer (API Controllers)
* Application Layer (Contracts, DTOs, Validation)
* Domain Layer (Entities and Business Models)
* Infrastructure Layer (Data Access, Services, Encryption)

The API supports authentication, account management, transactions, debit card operations, and administrative banking functions.

---

# Features

## Authentication & Authorization

* User registration
* User login
* JWT-based authentication
* Role-based authorization
* Profile retrieval
* Password hashing

## Account Management

* Automatic account creation during registration
* Account balance retrieval
* Account information retrieval
* Account freezing and unfreezing (Admin)

## Transaction Processing

* Deposits
* Withdrawals
* Internal transfers
* Inter-bank transfers
* Transaction history
* Transaction status tracking

## Debit Card Management

* Debit card issuance
* Card blocking and unblocking
* Card spending limits
* Card transaction processing
* Card transaction history
* PIN encryption using AES

## Administrative Operations

* View all users
* View all transactions
* Freeze accounts
* Unfreeze accounts

---

# Architecture

```text
BankingAPI
│
├── BankingAPI.Presentation
│   ├── Controllers
│   └── API Configuration
│
├── BankingAPI.Application
│   ├── DTOs
│   ├── Interfaces
│   └── Validators
│
├── BankingAPI.Domain
│   └── Entities
│
└── BankingAPI.Infrastructure
    ├── Data
    ├── Repositories
    ├── Services
    ├── Middleware
    └── Encryption
```

---

# Technology Stack

| Component         | Technology            |
| ----------------- | --------------------- |
| Framework         | ASP.NET Core 8        |
| Language          | C# 12                 |
| Database          | SQL Server            |
| ORM               | Entity Framework Core |
| Authentication    | JWT Bearer Tokens     |
| Documentation     | Swagger / OpenAPI     |
| Encryption        | AES                   |
| Password Security | SHA256                |
| Architecture      | N-Layer Architecture  |

---

# Domain Models

## User

Represents a banking customer or administrator.

## Account

Represents a customer's bank account.

### Properties

* Account Number
* Balance
* Status
* User Reference

## Transaction

Represents monetary movements.

### Types

* Deposit
* Withdrawal
* Transfer
* Inter-Bank Transfer

### Status

* Pending
* Successful
* Failed

## Debit Card

Represents a customer's debit card.

### Properties

* Card Number
* Expiry Date
* CVV
* Encrypted PIN
* Spending Limits
* Status

## Card Transaction

Tracks transactions performed using debit cards.

---

# API Endpoints

## Authentication

### Register

```http
POST /api/auth/register
```

### Login

```http
POST /api/auth/login
```

### Get Profile

```http
GET /api/auth/profile
```

Requires authentication.

---

## Accounts

### Get My Account

```http
GET /api/accounts/me
```

### Get Account Balance

```http
GET /api/accounts/balance
```

---

## Transactions

### Deposit

```http
POST /api/transactions/deposit
```

### Withdraw

```http
POST /api/transactions/withdraw
```

### Transfer

```http
POST /api/transactions/transfer
```

### Inter-Bank Transfer

```http
POST /api/transactions/inter-bank-transfer
```

### Transaction History

```http
GET /api/transactions/history
```

---

## Cards

### Issue Card

```http
POST /api/cards/issue
```

### Get My Cards

```http
GET /api/cards/my-cards
```

### Get Card Details

```http
GET /api/cards/{cardId}/details
```

### Block Card

```http
POST /api/cards/{cardId}/block
```

### Unblock Card

```http
POST /api/cards/{cardId}/unblock
```

### Update Spending Limits

```http
PUT /api/cards/{cardId}/limits
```

### Process Card Transaction

```http
POST /api/cards/{cardId}/transaction
```

### Card Transaction History

```http
GET /api/cards/{cardId}/transactions
```

---

## Administration

### Get All Users

```http
GET /api/admin/users
```

### Get All Transactions

```http
GET /api/admin/transactions
```

### Freeze Account

```http
PUT /api/admin/accounts/{id}/freeze
```

### Unfreeze Account

```http
PUT /api/admin/accounts/{id}/unfreeze
```

---

# Getting Started

## Prerequisites

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or VS Code

---

## Clone Repository

```bash
git clone https://github.com/DabereIbe/BankingAPI.git
cd BankingAPI
```

---

## Configure Database

Update the connection string inside:

```json
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BankingApiDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

---

## Configure JWT

```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "BankingAPI",
    "Audience": "BankingAPIUsers"
  }
}
```

---

## Restore Packages

```bash
dotnet restore
```

---

## Build

```bash
dotnet build
```

---

## Run

```bash
dotnet run
```

---

# Swagger Documentation

When the application starts, Swagger UI is available for interactive API testing.

Typical URL:

```text
https://localhost:5001/swagger
```

or

```text
https://localhost:{port}/swagger
```

---

# Security Features

* JWT Authentication
* Role-Based Authorization
* AES Encryption for Card PINs
* Secure Password Hashing
* Global Exception Handling Middleware
* Entity Framework SQL Injection Protection
* Request Validation

---

# Future Improvements

* Refresh Tokens
* Email Notifications
* SMS Alerts
* Multi-Factor Authentication
* Account Statements (PDF)
* Loan Management
* Savings Goals
* Fixed Deposits
* Audit Logging
* Rate Limiting
* Open Banking Integration

---

# License

This project is intended for educational, learning, and portfolio purposes.

Feel free to fork, modify, and extend it for your own projects.

---

# Author

**Dabere Ibe**

Software Engineer | Backend Developer | ASP.NET Core Developer

GitHub: https://github.com/DabereIbe
