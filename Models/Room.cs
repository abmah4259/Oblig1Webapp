using System.ComponentModel.DataAnnotations;

namespace StudyRoom.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public Building Building { get; set; }
        public RoomType RoomType { get; set; }
        public Equipment Equipment { get; set; }

        [Range(1, 100)]
        public int Capacity { get; set; }

        public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}