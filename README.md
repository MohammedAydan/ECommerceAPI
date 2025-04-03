# E-Commerce API

A comprehensive RESTful API built with .NET 8 for managing an e-commerce platform.

## Features

* User Authentication & Authorization
* Product Management
* Category Management
* Shopping Cart Operations
* Order Processing
* Checkout Flow
* Pagination Support
* Token-based Authentication with JWT
* Refresh Token Mechanism

## API Endpoints

### Authentication

```
POST /api/v1/User/signUp        - Register new user
POST /api/v1/User/signIn        - User login
POST /api/v1/User/refresh-token - Refresh authentication token
GET  /api/v1/User               - Get current user details
```

### Products

```
GET    /api/v1/Products                - List all products (paginated)
POST   /api/v1/Products                - Create new product
GET    /api/v1/Products/{id}           - Get product details
PUT    /api/v1/Products/{id}           - Update product
DELETE /api/v1/Products/{id}           - Delete product
GET    /api/v1/Products/category/{id}  - Get products by category
```

### Categories

```
GET    /api/v1/Categories              - List all categories (paginated)
POST   /api/v1/Categories              - Create new category
GET    /api/v1/Categories/{id}         - Get category details
PUT    /api/v1/Categories/{id}         - Update category
DELETE /api/v1/Categories/{id}         - Delete category
```

### Shopping Cart

```
GET    /api/v1/Carts                   - Get current user's cart
POST   /api/v1/Carts                   - Create or update cart
PUT    /api/v1/Carts/{cartId}          - Update specific cart
DELETE /api/v1/Carts/{cartId}          - Delete cart
```

### Orders

```
GET    /api/v1/Orders                  - List all orders (admin) or user orders (paginated)
POST   /api/v1/Orders                  - Create new order
GET    /api/v1/Orders/{id}             - Get order details
PUT    /api/v1/Orders/{id}             - Update order
DELETE /api/v1/Orders/{id}             - Delete order
GET    /api/v1/Orders/user             - Get current user's orders
GET    /api/v1/Orders/user/getItems    - Get current user's order items
```

### Checkout

```
POST   /api/v1/Checkout                - Process checkout
```

## Data Models

### Product

```json
{
  "productId": 0,
  "productName": "string",
  "description": "string",
  "price": 0.0,
  "categoryId": 0,
  "sku": "string",
  "stockQuantity": 0,
  "imageUrl": "string",
  "category": {
    "categoryId": 0,
    "categoryName": "string",
    "parentCategoryId": 0,
    "description": "string"
  }
}
```

### Category

```json
{
  "categoryId": 0,
  "categoryName": "string",
  "parentCategoryId": 0,
  "description": "string",
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
  ]
}
```

### Cart

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
        "price": 0.0
        // other product properties
      }
    }
  ]
}
```

### Order

```json
{
  "id": "string",
  "userId": "string",
  "totalAmount": 0.0,
  "orderItems": [
    {
      "orderItemId": 0,
      "orderId": 0,
      "productId": 0,
      "quantity": 0,
      "price": 0.0
    }
  ],
  "paymentMethod": "string",
  "createdAt": "2024-04-03T00:00:00Z",
  "updatedAt": "2024-04-03T00:00:00Z"
}
```

### User Registration

```json
{
  "userName": "string",
  "email": "user@example.com",
  "country": "string",
  "city": "string",
  "address": "string",
  "phoneNumber": "string",
  "password": "password",
  "confirmPassword": "password"
}
```

## Authentication

The API uses JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer {your-token}
```

To refresh an expired token, use the refresh token endpoint:

```
POST /api/v1/User/refresh-token
{
  "refreshToken": "your-refresh-token"
}
```

## Pagination

List endpoints support pagination with query parameters:
- `page`: Page number (default: 1)
- `limit`: Items per page (default: 10)

Example:
```
GET /api/v1/Products?page=2&limit=25
```

## Category Hierarchy

Categories support parent-child relationships through the `parentCategoryId` property. When retrieving a category, you can include its products by using the `getMyProducts` query parameter:

```
GET /api/v1/Categories/1?getMyProducts=true
```

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK` - Request succeeded
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication failed
- `403 Forbidden` - Authorization failed
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

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
