# StudyRoom – Study Room Booking Tool

ITPE3200 Web Applications, 2026H – Mandatory Assignment 1
Group: 
Members: Abbay, Helene, Aman

## About
A web application where students book study rooms on campus and join study sessions.
Admins manage the room inventory. Built with ASP.NET Core 10.0 MVC, Entity Framework Core and SQLite.

## Requirements
- .NET SDK 10.0
- No Node.js required (Subapp 1 is pure MVC)

## How to run
```bash
cd StudyRoomBooker
dotnet run
```
Open the URL shown in the terminal (e.g. `http://localhost:5136`).

The SQLite database (`studyroom.db`) is created automatically on first start,
with 10 rooms and the test accounts below.

## Test accounts
| Role    | Email               | Password      |
|---------|---------------------|---------------|
| Admin   | admin@studyroom.no  | Admin123!     |
| Student | kari@studyroom.no   | Student123!   |
| Student | ola@studyroom.no    | Student123!   |

- **Admin:** create, edit and delete rooms.
- **Student:** browse rooms, book study sessions, edit and cancel own bookings.

## Architecture
Models → Data (EF Core, SQLite) → Repositories → Services → Controllers → Views

- Business rules for bookings are in `Services/StudySessionService.cs`
  (max 4 hours, no room overlap, max 3 future bookings per student, edit lock 15 min before start).
- Global error handling and logging in `Middleware/ExceptionHandlingMiddleware.cs`.
