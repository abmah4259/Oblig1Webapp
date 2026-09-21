using System.ComponentModel.DataAnnotations;

namespace StudyRoom.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Building { get; set; } = string.Empty;

        [Range(1, 500)]
        public int Capacity { get; set; }

        [StringLength(200)]
        public string? Equipment { get; set; }

        public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}