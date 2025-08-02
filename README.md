# **VexaFit Backend**

This repository contains the backend services for the VexaFit application, a comprehensive fitness platform. It is built using a clean architecture approach with .NET, providing a scalable and maintainable foundation for the application's features. The solution also includes a React-based admin dashboard for managing the application's data.

## **Project Structure**

The solution is organized into several layers, each with a distinct responsibility, following the principles of Clean Architecture. This separation of concerns makes the application easier to develop, test, and maintain.

### **1\. Domain Layer (/Domain)**

This is the core of the application. It contains all the business logic, entities, and rules that are independent of any specific technology or framework.

* **Entities**: Plain C\# classes (Workout, Exercise, Category, etc.) that represent the core concepts of the application.  
* **Enums**: Enumerations (RoleEnum, CategoryTypeEnum, etc.) that define a set of named constants for use throughout the application.  
* **Common**: Shared constants and default settings used across the project.

### **2\. Application Layer (/Application)**

This layer contains the application-specific business logic. It orchestrates the data flow between the Domain and Infrastructure layers and implements the use cases of the application.

* **DTOs (Data Transfer Objects)**: Defines the shape of data that is sent to and from the API, acting as a contract for the API's consumers.  
* **Interfaces (IAppServices, IRepository, IUnitOfWork)**: Defines the contracts for services and data access patterns, abstracting the implementation details.  
* **Services (/AppServices)**: Contains the core logic for each feature (e.g., WorkoutService, CategoryService).  
* **Mapping (/Mapping)**: Uses AutoMapper profiles to define how objects are mapped between layers (e.g., from a Workout entity to a WorkoutDTO).

### **3\. Infrastructure Layer (/Infrastructure)**

This layer contains the implementation details for external concerns, such as databases, file systems, and other third-party services. It implements the interfaces defined in the Application layer.

* **Persistence (/Context, /Repository, /Migrations)**: Implements data access using Entity Framework Core, including the DbContext, repository pattern implementations, and database migrations.  
* **Services (/AppServices)**: Contains implementations of the application service interfaces.  
* **Seeds (/Seeds)**: Includes logic for seeding the database with initial data.  
* **Unit of Work (/UnitOfWork)**: Implements the Unit of Work pattern to manage transactions and ensure data consistency.

### **4\. Presentation Layer (/API)**

This is the entry point to the application. It's an ASP.NET Core Web API project responsible for handling HTTP requests, routing them to the appropriate services in the Application layer, and returning responses.

* **Controllers**: Handle incoming HTTP requests, validate input, call the appropriate application services, and return HTTP responses.  
* **Middleware**: Contains custom middleware for tasks like global exception handling.  
* **Program.cs & Startup.cs**: Configure the application's services, dependency injection, and request pipeline.

## **Tools and Packages**

### **Backend & API**

* **.NET & ASP.NET Core**: The core framework for building the web API.  
* **Entity Framework Core**: The object-relational mapper (O/RM) used for data access and communication with the SQL Server database.  
* **AutoMapper**: A library used for object-to-object mapping, simplifying the conversion between entities and DTOs.  
* **FluentValidation**: Used for building strongly-typed validation rules for DTOs.  
* **Serilog**: A structured logging library used for flexible and reliable application logging.  
* **JSON Web Tokens (JWT)**: Used for securing the API by implementing token-based authentication and authorization.

### **Admin Dashboard (/AdminDashboard)**

The admin dashboard is a single-page application (SPA) built with React for managing the application's data.

* **React**: The JavaScript library for building the user interface.  
* **Vite**: A fast front-end build tool that provides a quicker and more streamlined development experience.  
* **Axios**: A promise-based HTTP client for making API requests from the browser to the backend.  
* **react-router-dom**: Used for handling routing and navigation within the single-page application.

### **Testing**

The solution includes projects for both automated unit testing and performance testing to ensure code quality and application reliability.

#### **Unit & Integration Tests (/InfrastructureTests)**

* **xUnit**: The testing framework used to write and run unit tests for the application logic.  
* **Moq**: A popular mocking library for .NET, used to isolate parts of the application for testing by creating mock objects.  
* **Microsoft.EntityFrameworkCore.InMemory**: Used to create an in-memory database for fast and isolated testing of data access logic.

#### **Stress & Load Tests (/LoadTests)**

* **k6**: An open-source load testing tool used to test the performance and reliability of the API under heavy load. The StressTest.js file contains different scenarios that simulate various user behaviors (e.g., admin users, power users, browsing users) to identify performance bottlenecks.
