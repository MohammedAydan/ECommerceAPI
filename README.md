# E-Commerce API

A comprehensive RESTful API built with .NET 8 for managing an e-commerce platform.

## Features

* User Authentication & Authorization
* Product Management
* Category Management
* Shopping Cart Operations
* Order Processing
* Pagination Support
* Token-based Authentication
* Refresh Token Mechanism

## API Endpoints

### Authentication
```
POST /api/v1/User/signUp      - Register new user
POST /api/v1/User/signIn      - User login
POST /api/v1/User/refresh-token - Refresh authentication token
GET  /api/v1/User/{id}        - Get user details
```

### Products
```
GET    /api/v1/Products          - List all products (paginated)
POST   /api/v1/Products          - Create new product
GET    /api/v1/Products/{id}     - Get product details
PUT    /api/v1/Products/{id}     - Update product
DELETE /api/v1/Products/{id}     - Delete product
GET    /api/v1/Products/category/{id} - Get products by category
```

### Categories
```
GET    /api/v1/Categories        - List all categories (paginated)
POST   /api/v1/Categories        - Create new category
GET    /api/v1/Categories/{id}   - Get category details
PUT    /api/v1/Categories/{id}   - Update category
DELETE /api/v1/Categories/{id}   - Delete category
```

### Shopping Cart
```
GET    /api/v1/Carts/{userId}    - Get user's cart
POST   /api/v1/Carts             - Create new cart
PUT    /api/v1/Carts/{cartId}    - Update cart
DELETE /api/v1/Carts/{cartId}    - Delete cart
```

### Orders
```
GET    /api/v1/Orders            - List all orders (paginated)
POST   /api/v1/Orders            - Create new order
GET    /api/v1/Orders/{id}       - Get order details
PUT    /api/v1/Orders/{id}       - Update order
DELETE /api/v1/Orders/{id}       - Delete order
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
  "imageUrl": "string"
}
```

### Category
```json
{
  "categoryId": 0,
  "categoryName": "string",
  "parentCategoryId": 0,
  "description": "string"
}
```

### Order
```json
{
  "orderId": 0,
  "userId": "string",
  "orderDate": "2024-01-03T00:00:00Z",
  "totalAmount": 0.0,
  "orderItems": [
    {
      "orderItemId": 0,
      "orderId": 0,
      "productId": 0,
      "quantity": 0,
      "price": 0.0
    }
  ]
}
```

## Authentication

The API uses JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer {your-token}
```

## Pagination

List endpoints support pagination with query parameters:
- `page`: Page number (default: 1)
- `limit`: Items per page (default: 10)

## Coming Soon
- Payment Integration
- Advanced Search
- Real-time Inventory Updates

## Technical Stack
- .NET 8
- RESTful Architecture
- JWT Authentication
- Entity Framework Core
