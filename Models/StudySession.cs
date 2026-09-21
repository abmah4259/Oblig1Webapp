using System.ComponentModel.DataAnnotations;

namespace StudyRoom.Models
{
    public class StudySession
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid HostId { get; set; }
        public Student Host { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Topic { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(1, 100)]
        public int MaxParticipants { get; set; }

        public ICollection<Participation> Participations { get; set; } = new List<Participation>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}