using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudyRoom.Models;

namespace StudyRoom.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Rooms.Any())
            {
                return;
            }

            var rooms = new List<Room>
            {
                new() { Id = Guid.NewGuid(), Name = "P32-101", Building = Building.P32, RoomType = RoomType.Grupperom, Equipment = Equipment.Data | Equipment.Tavle, Capacity = 6 },
                new() { Id = Guid.NewGuid(), Name = "P32-102", Building = Building.P32, RoomType = RoomType.Grupperom, Equipment = Equipment.Data | Equipment.Tavle, Capacity = 8 },
                new() { Id = Guid.NewGuid(), Name = "P32-Aula", Building = Building.P32, RoomType = RoomType.Sal, Equipment = Equipment.Projektor | Equipment.Mikrofon, Capacity = 60 },
                new() { Id = Guid.NewGuid(), Name = "P35-101", Building = Building.P35, RoomType = RoomType.Grupperom, Equipment = Equipment.Data | Equipment.Tavle, Capacity = 6 },
                new() { Id = Guid.NewGuid(), Name = "P35-102", Building = Building.P35, RoomType = RoomType.Grupperom, Equipment = Equipment.Data | Equipment.Tavle, Capacity = 8 },
                new() { Id = Guid.NewGuid(), Name = "P35-201", Building = Building.P35, RoomType = RoomType.Seminarrom, Equipment = Equipment.Krittavle | Equipment.Mikrofon | Equipment.Storskjerm, Capacity = 20 },
                new() { Id = Guid.NewGuid(), Name = "P35-Aula", Building = Building.P35, RoomType = RoomType.Sal, Equipment = Equipment.Projektor | Equipment.Mikrofon, Capacity = 80 },
                new() { Id = Guid.NewGuid(), Name = "P48-101", Building = Building.P48, RoomType = RoomType.Grupperom, Equipment = Equipment.Data | Equipment.Tavle, Capacity = 6 },
                new() { Id = Guid.NewGuid(), Name = "P48-201", Building = Building.P48, RoomType = RoomType.Seminarrom, Equipment = Equipment.Krittavle | Equipment.Mikrofon | Equipment.Storskjerm, Capacity = 20 },
                new() { Id = Guid.NewGuid(), Name = "P48-202", Building = Building.P48, RoomType = RoomType.Seminarrom, Equipment = Equipment.Krittavle | Equipment.Mikrofon | Equipment.Storskjerm, Capacity = 25 }
            };

            context.Rooms.AddRange(rooms);
            context.SaveChanges();
        }

        public static async Task SeedIdentityDataAsync(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("Student"))
                await roleManager.CreateAsync(new IdentityRole("Student"));

            // Admin-konto
            const string adminEmail = "admin@studyroom.no";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Faste student-kontoer
            var seedStudents = new[]
            {
                new { Email = "kari@studyroom.no", Password = "Student123!", FirstName = "Kari", LastName = "Nordmann", StudentNumber = "S1001" },
                new { Email = "ola@studyroom.no",  Password = "Student123!", FirstName = "Ola",  LastName = "Hansen",   StudentNumber = "S1002" }
            };

            foreach (var s in seedStudents)
            {
                var identityUser = await userManager.FindByEmailAsync(s.Email);
                if (identityUser == null)
                {
                    identityUser = new IdentityUser { UserName = s.Email, Email = s.Email, EmailConfirmed = true };
                    await userManager.CreateAsync(identityUser, s.Password);
                    await userManager.AddToRoleAsync(identityUser, "Student");
                }

                if (!context.Students.Any(st => st.UserId == identityUser.Id))
                {
                    context.Students.Add(new Student
                    {
                        Id = Guid.NewGuid(),
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Email = s.Email,
                        StudentNumber = s.StudentNumber,
                        UserId = identityUser.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}