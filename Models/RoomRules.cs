using System.Linq;

namespace StudyRoom.Models
{
    public static class RoomRules
    {
        public static readonly Dictionary<Building, RoomType[]> AllowedRoomTypes = new()
        {
            { Building.P32, new[] { RoomType.Grupperom, RoomType.Sal } },
            { Building.P35, new[] { RoomType.Grupperom, RoomType.Seminarrom, RoomType.Sal } },
            { Building.P48, new[] { RoomType.Grupperom, RoomType.Seminarrom } }
        };

        public static readonly Dictionary<RoomType, Equipment[]> AllowedEquipment = new()
        {
            { RoomType.Grupperom, new[] { Equipment.Data, Equipment.Tavle } },
            { RoomType.Seminarrom, new[] { Equipment.Krittavle, Equipment.Mikrofon, Equipment.Storskjerm } },
            { RoomType.Sal, new[] { Equipment.Projektor, Equipment.Mikrofon } }
        };

        public static bool IsValidCombination(Building building, RoomType roomType, Equipment equipment)
        {
            if (!AllowedRoomTypes[building].Contains(roomType))
            {
                return false;
            }

            var allowed = AllowedEquipment[roomType];
            foreach (Equipment e in Enum.GetValues(typeof(Equipment)))
            {
                if (e == Equipment.None) continue;
                if (equipment.HasFlag(e) && !allowed.Contains(e))
                {
                    return false;
                }
            }
            return true;
        }
    }
}