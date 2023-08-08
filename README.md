# CarRentingSystem

CarRentingSystem is a web application for car renting and dealership management. It allows users to browse available cars, become car dealers, and book cars for rent. The application is built using ASP.NET Core 6.0 and follows the Model-View-Controller (MVC) architecture.

## Technologies Used

- ASP.NET Core 6.0
- Microsoft SQL Server (Database)
- Entity Framework Core (ORM)
- Razor Template Engine (View)
- HTML, CSS, and JavaScript (Front-End)
- Bootstrap (Front-End Styling)
- AutoMapper (Object Mapping)
- Microsoft.AspNetCore.Identity (User Authentication and Role Management)
- Dependency Injection (IoC Container)
- Unit Testing with Mocking
- TempData for displaying messages

## Features

- Car browsing: Users can browse and search for available cars.
- Car details: Users can view detailed information about a specific car, including its description, category, and dealer information.
- Car renting: Authenticated users can book a car for rent.
- Car dealership: Users can become car dealers by providing their dealership name and phone number.
- Role Management: Users have roles of User and Administrator. Administrators have additional privileges for managing the application.
- Data Validation: The application implements data validation on both the client-side and server-side to ensure data integrity and prevent invalid data entry.
- Security: The application handles security vulnerabilities like SQL injection, XSS, CSRF, parameter tampering, etc., to ensure a secure environment.
- Responsive Design: The application is designed to be responsive and works well on various devices, including tablets and smartphones.

## Test Accounts

You can use the following test accounts to log in and explore the application:

### Administrator Account:

| Email               | Password   |
|---------------------|------------|
| admin@crs.com       | admin123   |

### User Accounts:

| Email               | Password   |
|---------------------|------------|
|  mitko@gmail.com     | mitko@gmail.com   |
|  kristiyan@abv.bg    | kristiyan@abv.bg  |

### Dealers Accounts:

| Email               | Password   |
|---------------------|------------|
| tencho@yahoo.com    | tencho@yahoo.com   |
| vankata@yahoo.com   | vankata@yahoo.com  |
| sasho@gmail.com     | sasho@gmail.com  |

Please note that these accounts are for demonstration purposes only and may not have access to all features of the application. Feel free to create your own accounts or modify the existing ones to test different scenarios.

## Installation

1. Clone the repository.
2. Open the solution in Visual Studio 2022 or JetBrains Rider.
3. Make sure you have Microsoft SQL Server installed, and update the connection string in `appsettings.json` to point to your database.
4. Run the database migrations to create the required database schema using the Package Manager Console: `Update-Database`.
5. Build and run the application.

## Usage

1. Browse available cars on the home page.
2. Click on a car to view its details.
3. If you want to rent a car, you must be logged in. If you don't have an account, you can register as a new user.
4. After logging in, you can book a car for rent from the car details page.
5. If you want to become a car dealer, click on the "Become a Dealer" link in the navigation bar and fill in the required information.
6. Administrators have additional privileges for managing the application.

## Contributions

Contributions to the CarRentingSystem project are welcome. If you find any bugs or have suggestions for improvements, feel free to open an issue or submit a pull request.

## License

CarRentingSystem is licensed under the MIT License. See the LICENSE file for more details.
