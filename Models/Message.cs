using System.ComponentModel.DataAnnotations;

namespace StudyRoom.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }
        public StudySession StudySession { get; set; } = null!;

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime PostedAt { get; set; }
    }
}