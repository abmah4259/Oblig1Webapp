using Microsoft.EntityFrameworkCore;
using StudyRoom.Data;
using StudyRoom.Models;

namespace StudyRoom.Repositories
{
    public class StudySessionRepository : Repository<StudySession>, IStudySessionRepository
    {
        public StudySessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StudySession>> GetSessionsByRoomAsync(Guid roomId)
        {
            return await _dbSet
                .Where(s => s.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingSessionAsync(Guid roomId, DateTime start, DateTime end, Guid? excludeSessionId = null)
        {
            return await _dbSet.AnyAsync(s =>
                s.RoomId == roomId &&
                (excludeSessionId == null || s.Id != excludeSessionId) &&
                s.StartTime < end &&
                s.EndTime > start);
        }

        public async Task<bool> HasHostOverlappingSessionAsync(Guid hostId, DateTime start, DateTime end, Guid? excludeSessionId = null)
        {
            return await _dbSet.AnyAsync(s =>
                s.HostId == hostId &&
                (excludeSessionId == null || s.Id != excludeSessionId) &&
                s.StartTime < end &&
                s.EndTime > start);
        }

        public async Task<int> CountFutureBookingsByHostAsync(Guid hostId, Guid? excludeSessionId = null)
        {
            var now = DateTime.Now;
            return await _dbSet.CountAsync(s =>
                s.HostId == hostId &&
                (excludeSessionId == null || s.Id != excludeSessionId) &&
                s.StartTime > now);
        }
    }
}