# Flexible Authorization System with .NET 9 & Blazor

This project demonstrates a powerful and flexible permission-based authorization system built with .NET 9 for the backend API and Blazor WebAssembly for the client-side user interface.

The core of this system is a departure from traditional role-based authorization. While roles still exist to group users, they function as containers for permissions. Instead of checking if a user is in a role, we check if a user has a specific permission. Permissions are aggregated from all roles assigned to a user, providing granular control over access to application features. This approach avoids the common problem of "role explosion," where an application accumulates dozens of roles for minor variations in access rights.

## Table of Contents
- [Theoretical Foundation](#theoretical-foundation)
- [Key Features](#key-features)
- [Ideal Use Cases](#ideal-use-cases)
- [How It Works](#how-it-works)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Default Credentials](#default-credentials)
- [API Endpoints](#api-endpoints)

## Theoretical Foundation

This system implements a hybrid of Role-Based Access Control (RBAC) and Attribute-Based Access Control (ABAC), leveraging the strengths of both. At its heart, it is built on two core principles: bitwise permissions and claim-based authorization.

**Bitwise Permissions with Flag Enums**: Traditional authorization often involves lists of string-based permissions, which can be inefficient to store and query. This system uses a `[Flags]` enum, where each permission is assigned a value that is a power of two (1, 2, 4, 8, etc.). This mathematical approach allows any combination of permissions to be stored as a single integer. For example, a user with `ViewUsers` (4) and `ManageRoles` (2) would have a combined permission value of 6. Checking for a specific permission becomes a highly efficient bitwise AND operation, which is significantly faster than string comparisons or database queries.

**Stateless Authorization with JWT Claims**: A key performance bottleneck in many systems is the need to query the database on every API request to check a user's permissions. This project solves that by calculating a user's total aggregated permissions once upon login. This final integer value is then embedded directly into the user's JSON Web Token (JWT) as a custom claim. For the lifetime of that token, the API backend can perform authorization checks directly against this claim in-memory, making the process stateless and exceptionally fast. It also enforces the Principle of Least Privilege, as users carry only the permissions they are entitled to, and nothing more.

## Key Features

- **Permission-Based Control**: Define granular permissions for specific actions using a `[Flags]` enum. This approach allows individual permissions to be combined with simple bitwise operations, offering a highly efficient and flexible way to represent complex access rights.
- **Dynamic Policy Generation**: Authorization policies are created dynamically at runtime, eliminating the need to pre-register every permission combination. This prevents the common issue of bloating an application's startup configuration with hundreds of static policy definitions, making the authorization logic cleaner and easier to maintain.
- **Centralized Permission Management**: A user-friendly Blazor UI allows administrators to assign permissions to roles in a clear, matrix-style view. This provides a single source of truth for access control, making it easy to audit and update permissions without code changes.
- **Efficient Permission Checks**: A user's total permissions are calculated once upon login, aggregated from all their roles, and stored in a single JWT claim. This design ensures highly efficient validation on subsequent API requests, as it avoids repeated database lookups to check user roles.
- **Clean & Modern Tech Stack**:
    - **Backend**: .NET 9, EF Core, and ASP.NET Core Identity form a robust and scalable foundation.
    - **Frontend**: Blazor WebAssembly enables a rich, interactive single-page application (SPA) experience using C#.
    - **Authentication**: JWT (JSON Web Tokens) provide secure, stateless authentication between the client and server.
    - **API Client**: Refit is used for creating typed, declarative, and maintainable API clients, reducing boilerplate HTTP client code.
    - **Full Admin Interface**: Includes comprehensive management pages for Users, Roles, and Role Permissions, providing a complete, out-of-the-box solution for administering application access.

## Ideal Use Cases

This authorization model is particularly well-suited for applications requiring fine-grained control over user actions.

- **SaaS Platforms**: Manage feature gating based on subscription tiers (e.g., enabling `ExportData` or `AdvancedReporting` for Pro users).
- **Content Management Systems (CMS)**: Define precise publishing workflows (e.g., separating `CreateDraft` for Contributors from `PublishContent` for Editors).
- **Enterprise Resource Planning (ERP) Systems**: Control access to sensitive modules based on job function (e.g., `ViewFinancialReports` for Finance, `ManagePayroll` for HR).
- **E-commerce Platforms**: Delineate staff responsibilities (e.g., `ProcessRefunds` for Customer Support vs. `UpdateInventory` for Warehouse Staff).

## How It Works

The authorization logic is centered around a `Permissions` enum defined with the `[Flags]` attribute. This allows permissions to be combined using bitwise operations, with each permission having a value that is a power of two.

```csharp
// FlexibleAuthorization.Shared/Authorization/Permissions.cs
[Flags]
public enum Permissions
{
    None = 0,
    ViewRoles = 1, // 2^0
    ManageRoles = 2, // 2^1
    ViewUsers = 4, // 2^2
    ManageUsers = 8, // 2^3
    ConfigureAccessControl = 16, // 2^4
    Counter = 32, // 2^5
    Forecast = 64, // 2^6
    ViewAccessControl = 128, // 2^7
    All = ~None
}
```

- **Role Extension**: The standard `IdentityRole` is extended to include a `Permissions` property. By creating a custom `Role` class inheriting from `IdentityRole`, we seamlessly integrate our system into ASP.NET Core Identity's schema. This allows a combination of permission flags to be stored efficiently as a single integer in the database for each role.
- **Custom Claim Generation**: When a user logs in, permissions from all roles assigned to that user are aggregated using a bitwise OR operation. The combined integer value is then added as a single permissions claim to the user's JWT. This critical step ensures the JWT is a self-contained credential carrying all necessary authorization information. Note: While an `ApplicationUserClaimsPrincipalFactory` is provided for general claims principal creation, the login endpoint manually aggregates permissions for JWT generation.
- **Dynamic Policy Provider**: A custom `IAuthorizationPolicyProvider` (`FlexibleAuthorizationPolicyProvider`) intercepts authorization requests. Instead of looking for pre-registered policies, it parses the permission value from the policy name (e.g., "Permissions12") and dynamically builds an `AuthorizationPolicy` on the fly. This is the key to avoiding boilerplate registration in `Program.cs`.
- **Permission Handler**: The `PermissionAuthorizationHandler` contains the core validation logic. It reads the integer value from the user's permissions claim and performs a bitwise AND operation against the permission required by the policy. If the result matches the required permission, authorization succeeds.
- **Declarative Authorization**: This setup allows for simple, strongly-typed, and readable authorization checks in both the backend controllers and frontend Blazor components, significantly improving code maintainability.

**API Controller Example**:
```csharp
// This endpoint requires the user to have EITHER ViewUsers OR ManageUsers permission.
[HttpGet]
[Authorize(Permissions.ViewUsers | Permissions.ManageUsers)]
public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
{ //... }
```

**Blazor Component Example**:
```razor
@* This UI section is only visible to users with the ConfigureAccessControl permission. *@
<FlexibleAuthorizeView Permissions="Permissions.ConfigureAccessControl">
    <Authorized>
        <p>You can see this because you have the 'ConfigureAccessControl' permission!</p>
    </Authorized>
    <NotAuthorized>
        <p>You are not authorized to see this content.</p>
    </NotAuthorized>
</FlexibleAuthorizeView>
```

## Project Structure

The solution is divided into three logical projects to maintain a clean separation of concerns:

- **FlexibleAuthorization.Api**: The ASP.NET Core Web API backend. This project contains the controllers, database context, Identity configuration, and the custom authorization policy logic.
- **FlexibleAuthorization.Client**: The Blazor WebAssembly frontend application. It contains all UI components (Razor pages), API service clients generated by Refit, and local authentication state management.
- **FlexibleAuthorization.Shared**: A shared class library referenced by both the API and Client projects. It contains shared code such as DTOs, ViewModels, the `Permissions` enum, and custom attributes to ensure consistency across the stack.

## Getting Started

Follow these steps to get the application running locally.

### Prerequisites
- .NET 9 SDK (or newer)
- A SQL Server instance (LocalDB, Express, or any other edition is fine)
- An IDE like Visual Studio 2022 or VS Code

### Configuration

**API Connection String**: Open the `appsettings.json` file in the `FlexibleAuthorization.Api` project and update the `DefaultConnection` string to point to your SQL Server instance. The application will handle database creation and migrations automatically.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FlexibleAuthorization;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**API Base URL**: In the `FlexibleAuthorization.Client` project, verify the API base URL in `Program.cs`. By default, it is set to `https://localhost:7288`. Adjust the port if your API runs on a different one.

```csharp
// FlexibleAuthorization.Client/Program.cs
builder.Services.AddFlexibleAuthorizationClients("https://localhost:7288");
```

### Running the Application

1. **Launch the API**:
    - Navigate to the `FlexibleAuthorization.Api` directory.
    - Run `dotnet run --launch-profile "https"`.
    - The database will be automatically created and seeded with default roles and users on the first run.

2. **Launch the Client**:
    - In a new terminal, navigate to the `FlexibleAuthorization.Client` directory.
    - Run `dotnet run --launch-profile "https"`.
    - Open your browser and navigate to the Blazor application's URL (e.g., `https://localhost:7037`).

## Default Credentials

The database initializer creates two users by default to facilitate immediate testing. Use the admin user to log in and explore the application's full functionality.

- **Username**: `admin@localhost`
- **Password**: `Password123!`

The other user, auditor@localhost, is created without any assigned roles (zero permissions) to demonstrate how a user with no permissions interacts with the application.
- **Auditor Username**: `auditor@localhost`
- **Auditor Default Password**: `Password123!`

Note: Default roles include:
- **Administrators**: All permissions.
- **Accounts**: ViewUsers | Counter.
- **Operations**: ViewUsers | Forecast.

## API Endpoints

The API provides several endpoints for managing users, roles, and permissions. You can explore a full OpenAPI specification by running the API project and navigating to `/scalar/v1`.

- **POST /api/Auth/login**: Authenticates a user and returns a JWT.
- **GET /api/admin/Users**: Retrieves a list of all users.
- **GET /api/admin/Users/{id}**: Retrieves a specific user by ID.
- **PUT /api/admin/Users/{id}**: Updates a user's details and role assignments.
- **GET /api/admin/Roles**: Retrieves a list of all roles.
- **POST /api/admin/Roles**: Creates a new role.
- **PUT /api/admin/Roles/{id}**: Updates a role.
- **DELETE /api/admin/Roles/{id}**: Deletes a role.
- **GET /api/admin/AccessControl**: Gets the full configuration of roles and their assigned permissions.
- **PUT /api/admin/AccessControl**: Updates the permissions for a specific role.

