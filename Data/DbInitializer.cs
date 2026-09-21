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
    }
}