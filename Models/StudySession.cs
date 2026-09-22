using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyRoom.Models
{
    public class StudySession
    {
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }

        [ValidateNever]
        public Room Room { get; set; } = null!;

        public Guid HostId { get; set; }

        [ValidateNever]
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

        [ValidateNever]
        public ICollection<Participation> Participations { get; set; } = new List<Participation>();

        [ValidateNever]
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}