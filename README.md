<div align="center">
  <h1>🛍️ Bekam E-Commerce API</h1>
  <p>full-stack e-commerce platform built with <strong>ASP.NET Core</strong> and <strong>Angular</strong>.</p>

  

  <img src="https://img.shields.io/badge/.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Core" />
  <img src="https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Angular" />
  <img src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
</div>

<br/>

> ⚠️ **Public Repository Notice**
> This repository contains a **public, clean version** of the project. The original repository is private as it contains sensitive development history (environment configurations, credentials, etc.). Sensitive files and secrets have been removed here. The purpose of this repository is to demonstrate the **project architecture, code structure, and backend engineering practices**.

---

## 🚀  Test Account


To explore the **Admin Dashboard** and system capabilities without modifying data, you can log in using the Read-Only Admin account:
* **Email:** `ReadOnlyAdmin@Bekam.com`
* **Password:** `ReadOnlyAdmin@123`

---

## 🎯 Project Overview

**Bekam** is an advanced backend API responsible for powering a modern e-commerce platform. It handles everything from secure authentication and product catalog management to background processing and performance optimizations.

**System Highlights:**
* **Smart Cart System:** Anonymous cart functionality using Redis with automatic merging upon user login.
* **Granular Security:** Advanced Permission-based authorization system (Roles + Permissions).
* **Payment Processing:** Seamless Stripe payment integration for secure transactions.
* **Background Jobs:** Asynchronous job processing utilizing Hangfire.
* **Clean Architecture:** Strict adherence to Onion Architecture with Repository, UnitOfWork, and Specification patterns.
* **Enterprise Infrastructure:** Includes Redis caching, IP/User rate limiting, structured logging, and email services.

---

## 🏗 Architecture & Infrastructure

### Onion Architecture
The backend strictly follows **Onion Architecture**, ensuring business logic is isolated from external frameworks and infrastructure. This greatly improves maintainability, scalability, and testability.

```
Bekam.API
Bekam.Application
Bekam.Domain
Bekam.Infrastructure
```

### Infrastructure Capabilities
The system is built for reliability and scalability using industry-standard tools:
* **FluentValidation:** Robust API request validation.
* **Mapster:** High-performance object mapping.
* **MailKit:** Reliable email dispatch services.
* **Serilog:** Structured, easily queryable logging.
* **Rate Limiting:** Protection via UserId and IP address tracking.
* **Global Error Handling:** Consistent, informative error responses.
* **Result Pattern:** Standardized wrapper for API responses.
* **Database Seeding:** Automatic generation of core roles, permissions, and admin users.

---

## 💻 Technology Stack

**Framework & Core:**
* ASP.NET Core Web API
* Entity Framework Core
* ASP.NET Core Identity

**Databases & Caching:**
* Microsoft SQL Server
* Redis

**Background & Utilities:**
* Hangfire (Background Processing)
* Stripe (Payment Gateway)
* Serilog (Logging)
* MailKit (Email)

**Design Patterns:**
* Onion Architecture
* Repository & Unit of Work Pattern
* Specification Pattern
* Result Pattern

---

## ✨ Core Features

### 👤 Guest Experience
The platform embraces frictionless shopping by supporting anonymous users.
* Browse products, categories, and brands freely.
* Add items to an anonymous cart powered by Redis caching (stored for 7 days).
* **Intelligent Merge:** If a guest registers or logs in, the system automatically merges their anonymous Redis cart with their persistent database account.

### 🔐 Authentication & Authorization
Powered by ASP.NET Core Identity, JWT Tokens, and Refresh Tokens.
* Full flow: Registration, Email Confirmation, Login, Forgot/Reset Password.
* Newly registered users automatically receive the Member role.

### 🛡️ Role & Permission System
Granular access control using Role-Based and Permission-Based structures.
* **Admin:** Full system control (Products, Categories, Brands, Users, Roles, Permissions). Can assign roles, grant permissions, and lock/unlock accounts.
* **ReadonlyAdmin:** Designed specifically for demonstration. Has access to view dashboard data (products:get, categories:get, roles:get, users:get) but strictly prevented from creating, updating, deleting, or altering passwords.
* **Member:** Standard authenticated shoppers.

### 📦 Product & Catalog Management
Admins have full CRUD control over Products, Categories, and Brands. The API utilizes the Specification Pattern to execute highly flexible and reusable queries for filtering, sorting, and pagination.

### 💳 Orders & Payments
Authenticated Members can smoothly transition from cart management to secure checkout. Complete Stripe integration handles the financial transactions securely, while the system tracks comprehensive order histories.

### ⚙️ Background Processing & Caching
* **Hangfire:** Offloads heavy tasks to background queues, ensuring the API remains highly responsive (e.g., sending confirmation emails, scheduled cleanup tasks).
* **Redis:** Drastically reduces database load by caching anonymous carts and heavily queried catalog data.

---

## 📸 Application Screens

### 🛒 Storefront Interface
<table>
<tr>
<td colspan="2" align="center">
<b>Home Page (Hero Section)</b><br/>
<img src="https://github.com/user-attachments/assets/599a01f3-80ef-4940-b9fe-82f6850ee24e" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Home Page (Middle)</b><br/>
<img src="https://github.com/user-attachments/assets/e56fbdf0-dc70-434f-8039-48aceb259a01" width="100%"/>
</td>
<td width="50%" align="center">
<b>Home Page (Bottom)</b><br/>
<img src="https://github.com/user-attachments/assets/b6220732-db56-4973-98c1-b0502c4134c5" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Product Listing</b><br/>
<img src="https://github.com/user-attachments/assets/ee017bdd-073e-45f4-b4a0-a6d583da9383" width="100%"/>
</td>
<td width="50%" align="center">
<b>Product Details</b><br/>
<img src="https://github.com/user-attachments/assets/a9621cf1-3064-4c68-8091-87242f8853d0" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Categories</b><br/>
<img src="https://github.com/user-attachments/assets/cd712190-df73-4540-85a2-e9ef73292ddd" width="100%"/>
</td>
<td width="50%" align="center">
<b>Sub Categories</b><br/>
<img src="https://github.com/user-attachments/assets/34c53d6b-c499-4c85-8b35-74f4e2c1841f" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Brands</b><br/>
<img src="https://github.com/user-attachments/assets/fbb7eab4-2cb5-4bca-b52f-f401e09ef2d5" width="100%"/>
</td>
<td width="50%" align="center">
<b>Cart Management</b><br/>
<img src="https://github.com/user-attachments/assets/5378c1dc-550c-4ae7-990e-8be0775ff0bf" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Checkout (Step 1)</b><br/>
<img src="https://github.com/user-attachments/assets/353ef291-4bb0-4272-a761-ce7ce2519223" width="100%"/>
</td>
<td width="50%" align="center">
<b>Checkout (Step 2)</b><br/>
<img src="https://github.com/user-attachments/assets/54426803-31db-43bf-b80e-9bac39493874" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Orders</b><br/>
<img src="https://github.com/user-attachments/assets/85bf58da-d710-4ade-8f18-3ac4c004c231" width="100%"/>
</td>
<td width="50%" align="center">
<b>Profile Page</b><br/>
<img src="https://github.com/user-attachments/assets/2557280d-19f2-4919-ae6f-843a701e9b8b" width="100%"/>
</td>
</tr>
<tr>
<td colspan="2" align="center">
<b>Change Password Page</b><br/>
<img src="https://github.com/user-attachments/assets/db6be650-9e98-4f67-b1b0-12349cd7f8a1" width="50%"/>
</td>
</tr>
</table>

<br/>

### ⚙️ Admin Dashboard Interface
<table>
<tr>
<td colspan="2" align="center">
<b>Admin Dashboard Overview</b><br/>
<img src="https://github.com/user-attachments/assets/51acb8db-2677-4aab-aff2-b00d48adb386" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Manage Products</b><br/>
<img src="https://github.com/user-attachments/assets/e383bd19-2271-4fe4-8bcd-97d1880f2c55" width="100%"/>
</td>
<td width="50%" align="center">
<b>Manage Categories</b><br/>
<img src="https://github.com/user-attachments/assets/a76c6fed-7c6d-451a-922e-681aed1a889a" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Manage Brands</b><br/>
<img src="https://github.com/user-attachments/assets/4b89637d-f841-4bcb-9d18-5a2e5b35810f" width="100%"/>
</td>
<td width="50%" align="center">
<b>Roles & Permissions (List)</b><br/>
<img src="https://github.com/user-attachments/assets/fbbb9de6-55e6-4328-a277-e68280592875" width="100%"/>
</td>
</tr>
<tr>
<td width="50%" align="center">
<b>Roles & Permissions (assign permissions to role)</b><br/>
<img src="https://github.com/user-attachments/assets/2725b0fe-880d-4413-8ffc-571feecf09df" width="100%"/>
</td>
<td width="50%" align="center">
<b>Manage Users (List)</b><br/>
<img src="https://github.com/user-attachments/assets/706f4e88-6fbd-4c34-b570-25432b4372bf" width="100%"/>
</td>
</tr>
<tr>
<td colspan="2" align="center">
<b>Manage Users (assign roles to user)</b><br/>
<img src="https://github.com/user-attachments/assets/b9f1f5b6-76d3-4b33-adea-2154484fad46" width="50%"/>
</td>
</tr>
</table>
