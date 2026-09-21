using StudyRoom.Models;

namespace StudyRoom.Repositories
{
    public interface IStudySessionRepository : IRepository<StudySession>
    {
        Task<IEnumerable<StudySession>> GetSessionsByRoomAsync(Guid roomId);
        Task<bool> HasOverlappingSessionAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeSessionId = null);
    }
}