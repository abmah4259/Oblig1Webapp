using StudyRoom.Models;

namespace StudyRoom.Repositories
{
    public interface IStudySessionRepository : IRepository<StudySession>
    {
        Task<IEnumerable<StudySession>> GetAllWithDetailsAsync();
        Task<StudySession?> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<StudySession>> GetSessionsByRoomAsync(Guid roomId);
        Task<bool> HasOverlappingSessionAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeSessionId = null);
        Task<bool> HasHostOverlappingSessionAsync(Guid hostId, DateTime start, DateTime end, Guid? excludeSessionId = null);
        Task<int> CountFutureBookingsByHostAsync(Guid hostId, Guid? excludeSessionId = null);
    }
}