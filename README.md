# Bank Management System - C# WinForms
<img width="465" height="381" alt="image" src="https://github.com/user-attachments/assets/0cea9791-3941-4dff-a505-947be9a532d3" /><br/>
<img width="1362" height="702" alt="image" src="https://github.com/user-attachments/assets/bf46b921-ad46-4191-a495-30f47afff793" /><br/>
<img width="1366" height="714" alt="image" src="https://github.com/user-attachments/assets/0751e1cb-8649-475a-b714-d248dca7fbbf" /><br/>
<img width="1366" height="656" alt="image" src="https://github.com/user-attachments/assets/627e9987-8d73-4081-a437-db439aae7da9" /><br/>
<img width="1366" height="699" alt="image" src="https://github.com/user-attachments/assets/567fe4f4-4e14-4c59-8fde-b752349f7d3b" /><br/>
<img width="1366" height="699" alt="image" src="https://github.com/user-attachments/assets/64eeb50a-27e3-4e0b-9714-ea0231837a42" /><br/>
<img width="1182" height="633" alt="image" src="https://github.com/user-attachments/assets/2e956b40-326f-4a3d-99bb-297b8c91f1c5" /><br/>
<img width="1167" height="672" alt="image" src="https://github.com/user-attachments/assets/ea7bbafb-7857-44cf-9e32-a5a6c931b35e" /><br/>
<img width="1176" height="691" alt="image" src="https://github.com/user-attachments/assets/498b06d3-612a-43be-9632-cf052d8637f8" /><br/>
<img width="1170" height="651" alt="image" src="https://github.com/user-attachments/assets/689657e8-a2c4-4ecd-b7fa-80a5400fbad2" /><br/>
<img width="1170" height="651" alt="image" src="https://github.com/user-attachments/assets/6953e549-3b37-4465-8fb4-59e25e88aea3" /><br/>
<img width="1174" height="640" alt="image" src="https://github.com/user-attachments/assets/02194167-e912-43c1-9496-9aec716d8e73" /><br/>
<img width="1170" height="643" alt="image" src="https://github.com/user-attachments/assets/a3881389-2aa5-41d4-91ab-d32f735e1c86" />

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
