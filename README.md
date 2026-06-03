# Intern Portal

A modern ASP.NET Core MVC web application designed for managing intern rosters, coordinating with staff mentors, and tracking daily attendance records.

## Features

- **Dashboard**: Real-time stats showing total interns, present today count, total mentors, and the daily attendance rate.
- **Interns Directory**: Roster management with search filtering by name and dropdown filters by Mentor, Department, and Branch.
- **Relational Architecture**: Relational database structure linking Interns, Mentors, and daily Attendance records.
- **Daily Attendance**: Dedicated date-picker interface for recording daily presence with built-in validation to prevent duplicate entries for the same day.
- **Auto-Seeding**: Automatic seed database functionality that populates a list of 9 default staff mentors on the first launch.
- **IOCL Theme Design**: Upgraded layout using a responsive side navigation bar, Google Fonts (Poppins), and custom colors matching the IOCL brand style guidelines.

## Technologies Used

- **Framework**: .NET 9.0 ASP.NET Core MVC
- **Database**: SQLite (managed with Entity Framework Core)
- **Design & Icons**: Vanilla CSS, Bootstrap 5.3, Bootstrap Icons
- **Fonts**: Google Fonts (Poppins)

## Prerequisites

Ensure you have the following installed on your system:
- .NET 9.0 SDK
- Entity Framework Core CLI tool (`dotnet-ef`)

## Database Configuration

The application uses Entity Framework Core to communicate with SQLite.

1. **Verify EF Core CLI installation**:
   ```bash
   dotnet ef
   ```
   If not installed, install it globally using:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Database Migrations and Schema Updates**:
   To apply migrations and generate the SQLite database file:
   ```bash
   dotnet ef database update
   ```
   This will automatically create the SQLite database file (`interns_v2.db`) in the root directory and apply the schema.

## Getting Started

Follow these steps to run the application locally:

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access the application**:
   Open your browser and navigate to the localhost port listed in the console output (e.g., `http://localhost:5000` or `http://localhost:5244`).
