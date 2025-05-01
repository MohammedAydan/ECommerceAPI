# E-Commerce API Documentation

A comprehensive RESTful API built with .NET 8 for managing an e-commerce platform.

## Table of Contents
- [Overview](#overview)
- [Authentication](#authentication)
- [Products](#products)
- [Categories](#categories)
- [Shopping Cart](#shopping-cart)
- [Orders](#orders)
- [Checkout](#checkout)
- [Error Handling](#error-handling)
- [Pagination](#pagination)
- [Technical Stack](#technical-stack)
- [Getting Started](#getting-started)

## Overview

This API provides a complete solution for e-commerce applications with the following features:

- User Authentication & Authorization
- Product Management
- Category Management
- Shopping Cart Operations
- Order Processing
- Checkout Flow
- Pagination Support
- Token-based Authentication with JWT
- Refresh Token Mechanism
- File Upload Support for Product and Category Images

## Authentication

Authentication is handled using JWT (JSON Web Tokens) with a refresh token mechanism.

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/User/signUp` | Register new user |
| POST | `/api/v1/User/signIn` | User login |
| POST | `/api/v1/User/refresh-token` | Refresh authentication token |
| GET | `/api/v1/User` | Get current user details |
| PUT | `/api/v1/User/update` | Update user information |

### User Registration Model

```json
{
  "userName": "string",
  "email": "user@example.com",
  "country": "string",
  "city": "string",
  "address": "string",
  "phoneNumber": "string",
  "password": "password",
  "confirmPassword": "password",
  "image": "[binary file]"
}
```

### Login Model

```json
{
  "email": "user@example.com",
  "password": "password"
}
```

### Refresh Token Model

```json
{
  "refreshToken": "your-refresh-token"
}
```

### Authentication Header

Include the JWT token in the Authorization header for all protected endpoints:

```
Authorization: Bearer {your-token}
```

## Products

Products represent the items available for purchase in the e-commerce platform.

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Products` | List all products (paginated) |
| GET | `/api/v1/Products/top` | Get top products (paginated) |
| POST | `/api/v1/Products` | Create new product |
| GET | `/api/v1/Products/{id}` | Get product details |
| PUT | `/api/v1/Products/{id}` | Update product |
| DELETE | `/api/v1/Products/{id}` | Delete product |
| GET | `/api/v1/Products/category/{id}` | Get products by category |

### Product Model

```json
{
  "productId": 0,
  "productName": "string",
  "description": "string",
  "price": 0.0,
  "categoryId": 0,
  "sku": "string",
  "stockQuantity": 0,
  "image": "[binary file]",
  "imageUrl": "string",
  "discount": 0,
  "rating": 0,
  "salePrice": 0.0,
  "cartAddedCount": 0,
  "createdOrderCount": 0,
  "category": {
    "categoryId": 0,
    "categoryName": "string",
    "description": "string",
    "imageUrl": "string"
  }
}
```

## Categories

Categories are used to organize products into logical groups.

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Categories` | List all categories (paginated) |
| GET | `/api/v1/Categories/top` | Get top-level categories (paginated) |
| POST | `/api/v1/Categories` | Create new category |
| GET | `/api/v1/Categories/{id}` | Get category details |
| PUT | `/api/v1/Categories/{id}` | Update category |
| DELETE | `/api/v1/Categories/{id}` | Delete category |

### Category Model

```json
{
  "categoryId": 0,
  "categoryName": "string",
  "description": "string",
  "image": "[binary file]",
  "imageUrl": "string",
  "products": [
    {
      "productId": 0,
      "productName": "string",
      "description": "string",
      "price": 0.0,
      "categoryId": 0,
      "sku": "string",
      "stockQuantity": 0,
      "imageUrl": "string"
    }
  ],
  "itemsCount": 0
}
```

### Retrieving Category with Products

To include products when retrieving a category, add the `getMyProducts` parameter:

```
GET /api/v1/Categories/1?getMyProducts=true
```

## Shopping Cart

The shopping cart allows users to collect products before proceeding to checkout.

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Carts` | Get current user's cart |
| POST | `/api/v1/Carts/add` | Add product to cart |
| DELETE | `/api/v1/Carts/remove` | Remove product from cart |
| DELETE | `/api/v1/Carts/{cartId}` | Delete entire cart |

### Cart Model

```json
{
  "cartId": 0,
  "userId": "string",
  "cartItems": [
    {
      "cartItemId": 0,
      "productId": 0,
      "quantity": 0,
      "product": {
        "productId": 0,
        "productName": "string",
        "price": 0.0,
        "imageUrl": "string"
        // other product properties
      }
    }
  ]
}
```

### Adding Product to Cart

```
POST /api/v1/Carts/add
Content-Type: application/json

{productId}
```

### Removing Product from Cart

```
DELETE /api/v1/Carts/remove
Content-Type: application/json

{productId}
```

To remove all items from cart:

```
DELETE /api/v1/Carts/remove?removeAll=true
```

## Orders

Orders represent completed purchases by users.

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/Orders` | List all orders (admin) or user orders (paginated) |
| POST | `/api/v1/Orders` | Create new order |
| GET | `/api/v1/Orders/{id}` | Get order details |
| PUT | `/api/v1/Orders/{id}` | Update order |
| DELETE | `/api/v1/Orders/{id}` | Delete order |
| GET | `/api/v1/Orders/user` | Get current user's orders |
| GET | `/api/v1/Orders/user/getItems` | Get current user's order items |

### Order Model

```json
{
  "id": "string",
  "userId": "string",
  "totalAmount": 0.0,
  "orderItems": [
    {
      "id": "string",
      "orderId": "string",
      "productId": 0,
      "quantity": 0,
      "price": 0.0,
      "product": {
        // product details
      }
    }
  ],
  "paymentMethod": "string",
  "shippingAddress": "string",
  "status": "string",
  "createdAt": "2024-04-03T00:00:00Z",
  "updatedAt": "2024-04-03T00:00:00Z"
}
```

### Get Order with Items and Products

To include order items and their associated products when retrieving an order:

```
GET /api/v1/Orders/{id}?getMyItemsAndProducts=true
```

## Checkout

The checkout process finalizes the user's purchase.

### Endpoint

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/Checkout` | Process checkout |

### Checkout Request Model

```json
{
  "paymentMethod": "string",
  "shippingAddress": "string"
}
```

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK` - Request succeeded
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication failed
- `403 Forbidden` - Authorization failed
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Pagination

Most list endpoints support pagination with the following query parameters:

- `page`: Page number (default: 1)
- `limit`: Items per page (default: 10)

Example:
```
GET /api/v1/Products?page=2&limit=25
```

## Technical Stack

- .NET 8
- RESTful Architecture
- JWT Authentication
- Entity Framework Core
- Swagger API Documentation

## Getting Started

1. Clone the repository
2. Configure your database connection in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Run the application: `dotnet run`
5. Access Swagger documentation at: `https://localhost:5001/swagger`

## Additional Notes

### File Uploads

For endpoints that accept file uploads (such as product and category creation/updates), use `multipart/form-data` as the content type.

### Security

This API uses JWT authentication. All protected endpoints require a valid JWT token in the Authorization header.
