# E-Commerce API Documentation

## Table of Contents
- [Overview](#overview)
- [Authentication](#authentication)
- [Products](#products)
- [Categories](#categories)
- [Shopping Cart](#shopping-cart)
- [Orders](#orders)
- [Checkout](#checkout)
- [Dashboard](#dashboard)
- [User Management](#user-management)
- [Error Handling](#error-handling)
- [Pagination & Filtering](#pagination--filtering)
- [Technical Stack](#technical-stack)
- [Getting Started](#getting-started)

## Overview

This API provides a complete solution for e-commerce applications with the following features:

- JWT Authentication with Refresh Tokens
- Comprehensive Product Management
- Category Hierarchy Support
- Shopping Cart Functionality
- Order Processing System
- Checkout Flow
- Admin Dashboard Statistics
- User Profile Management
- Advanced Search and Filtering
- Pagination Support
- File Upload Capabilities

## Authentication

### Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/v1/User/signUp` | Register new user | Multipart form-data |
| POST | `/api/v1/User/signIn` | User login | JSON: `{email, password}` |
| POST | `/api/v1/User/refresh-token` | Refresh JWT token | JSON: `{refreshToken}` |
| GET | `/api/v1/User` | Get current user details | - |
| PUT | `/api/v1/User/update` | Update user information | Multipart form-data |

### User Registration (Multipart Form)

Required fields:
- UserName (string)
- Email (valid email format)
- Password (8-50 characters)
- ConfirmPassword (must match Password)
- Country (string)
- City (string)
- Address (string)
- Image (binary file, optional)

### Authentication Header

```
Authorization: Bearer {your-jwt-token}
```

## Products

### Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/v1/Products` | List products | page, limit, search, sortBy, ascending |
| GET | `/api/v1/Products/top` | Get top products | page, limit |
| GET | `/api/v1/Products/category/{id}` | Products by category | id (string), page, limit |
| GET | `/api/v1/Products/{id}` | Product details | id (integer) |
| GET | `/api/v1/Products/search` | Search products | searchTerm, page, limit, categoryId, minPrice, maxPrice |
| POST | `/api/v1/Products` | Create product | Multipart form |
| PUT | `/api/v1/Products/{id}` | Update product | id (integer), Multipart form |
| DELETE | `/api/v1/Products/{id}` | Delete product | id (integer) |

### Product Search Parameters

- `searchTerm`: String to search in product names/descriptions
- `categoryId`: Filter by category
- `minPrice`/`maxPrice`: Price range filter
- `page`/`limit`: Pagination controls
- `sortBy`: Field to sort by (default: "Id")
- `ascending`: Sort direction (default: true)

## Categories

### Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/v1/Categories` | List categories | page, limit, search, sortBy, ascending |
| GET | `/api/v1/Categories/top` | Top categories | page, limit |
| GET | `/api/v1/Categories/simple` | Simplified category list | page, limit |
| GET | `/api/v1/Categories/{id}` | Category details | id (integer), getMyProducts, page, limit |
| POST | `/api/v1/Categories` | Create category | Multipart form |
| PUT | `/api/v1/Categories/{id}` | Update category | id (integer), Multipart form |
| DELETE | `/api/v1/Categories/{id}` | Delete category | id (integer) |

## Shopping Cart

### Endpoints

| Method | Endpoint | Description | Parameters/Request |
|--------|----------|-------------|---------------------|
| GET | `/api/v1/Carts` | Get user's cart | - |
| POST | `/api/v1/Carts/add` | Add to cart | JSON: productId (integer) |
| DELETE | `/api/v1/Carts/remove` | Remove from cart | JSON: productId (integer), Query: removeAll (boolean) |
| DELETE | `/api/v1/Carts/{cartId}` | Delete cart | cartId (integer) |

## Orders

### Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/v1/Orders` | List orders | page, limit, search, sortBy, ascending |
| GET | `/api/v1/Orders/user` | User orders | page, limit |
| GET | `/api/v1/Orders/user/with-items` | Orders with items | page, limit, pageItems, limitItems |
| GET | `/api/v1/Orders/{id}` | Order details | id (string), includeProducts, page, limit |
| POST | `/api/v1/Orders` | Create order | JSON: CheckoutParams |
| PUT | `/api/v1/Orders/{id}` | Update order | id (string), JSON: OrderDTO |
| DELETE | `/api/v1/Orders/{id}` | Delete order | id (string) |

## Checkout

### Endpoint

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| POST | `/api/v1/Checkout` | Process checkout | JSON: CheckoutRequestDTO |

### Checkout Models

**CheckoutRequestDTO:**
```json
{
  "paymentMethod": "string",
  "shippingAddress": "string",
  "shippingPrice": 0.0
}
```

**CheckoutParams:**
```json
{
  "paymentMethod": "string",
  "shippingAddress": "string",
  "shippingPrice": 0.0
}
```

## Dashboard

### Endpoint

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Dashboard/stats` | Get dashboard statistics |

**DashboardStats Model:**
```json
{
  "totalRevenue": 0.0,
  "totalOrders": 0,
  "totalProducts": 0,
  "activeUsers": 0,
  "revenueGrowth": 0.0,
  "ordersGrowth": 0,
  "productsGrowth": 0,
  "usersGrowth": 0
}
```

## User Management

### Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/v1/User/all` | List all users | page, limit, search, sortBy, ascending |

## Error Handling

Standard HTTP status codes are used with JSON error responses:

- `200 OK`: Successful request
- `400 Bad Request`: Invalid input/data
- `401 Unauthorized`: Authentication required
- `403 Forbidden`: Insufficient permissions
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server error

## Pagination & Filtering

Most list endpoints support:

**Pagination:**
- `page`: Current page (default: 1)
- `limit`: Items per page (default: 10)

**Filtering/Sorting:**
- `search`: Text search filter
- `sortBy`: Field to sort by (default varies)
- `ascending`: Sort direction (default: true)

**Nested Pagination:**
Some endpoints support nested pagination for related items:
- `pageItems`: Page number for nested items
- `limitItems`: Items per page for nested items

## Technical Stack

- .NET 8
- RESTful API Design
- JWT Authentication
- Entity Framework Core
- Swagger/OpenAPI 3.0
- Multipart File Uploads
- Pagination & Filtering
- Clean Architecture

## Getting Started

1. Ensure .NET 8 SDK is installed
2. Configure database connection strings
3. Set JWT secret key in configuration
4. Run database migrations
5. Launch application
6. Access Swagger UI at `/swagger`

## File Uploads

For endpoints requiring file uploads (products/categories/users):
- Use `multipart/form-data` content type
- File fields are marked as `format: binary`
- Other fields should be sent as form fields

## Security

All endpoints (except auth-related) require JWT authentication via:
```
Authorization: Bearer {token}
```

Refresh tokens can be used to obtain new access tokens when they expire.
