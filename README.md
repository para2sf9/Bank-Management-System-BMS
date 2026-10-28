# Bank Management System - C# WinForms

A desktop GUI application built with C# WinForms and MongoDB.

## Requirements
- .NET 8.0 SDK or later
- Windows OS
- MongoDB running locally or remotely

## Setup
1. Build the project:
   ```
   dotnet build
   ```

2. Run the application:
   ```
   dotnet run
   ```

## Features
- Login with role-based access
- Employee CRUD (with soft delete)
- Department management
- Attendance tracking
- Payroll generation
- Dashboard with statistics
- MongoDB connection pooling and indexing

## Default Login
- Username: `admin`
- Password: `admin123`

## Project Structure
- `Program.cs` - Entry point
- `DatabaseService.cs` - MongoDB service layer with async operations
- `LoginForm.cs` - Login window
- `MainForm.cs` - Main dashboard with sidebar navigation
- `DashboardPanel.cs`, `EmployeesPanel.cs`, `DepartmentsPanel.cs`, `AttendancePanel.cs`, `PayrollPanel.cs` - Feature modules
- `EmployeeForm.cs`, `DepartmentForm.cs`, `MarkAttendanceForm.cs`, `GeneratePayrollForm.cs` - Data entry dialogs
