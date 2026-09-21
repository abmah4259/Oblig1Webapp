using System.ComponentModel.DataAnnotations;

namespace StudyRoom.Models
{
    public class Student
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

       // [Required]
        //Fjerner Denne linjen fra student.cs
      //  public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string StudentNumber { get; set; } = string.Empty;

        public ICollection<StudySession> HostedSessions { get; set; } = new List<StudySession>();
        public ICollection<Participation> Participations { get; set; } = new List<Participation>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}