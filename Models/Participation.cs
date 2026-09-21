namespace StudyRoom.Models
{
    public class Participation
    {
        public Guid Id { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public Guid SessionId { get; set; }
        public StudySession StudySession { get; set; } = null!;

        public DateTime JoinedAt { get; set; }
    }
}