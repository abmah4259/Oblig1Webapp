using StudyRoom.Models;
using StudyRoom.Repositories;

namespace StudyRoom.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly IStudySessionRepository _sessionRepository;

        public StudySessionService(IStudySessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<StudySession>> GetAllAsync()
        {
            return await _sessionRepository.GetAllAsync();
        }

        public async Task<StudySession?> GetByIdAsync(Guid id)
        {
            return await _sessionRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(StudySession session)
        {
            if (session.EndTime <= session.StartTime)
            {
                return (false, "Sluttidspunkt må være etter starttidspunkt.");
            }

            var overlaps = await _sessionRepository.HasOverlappingSessionAsync(
                session.RoomId, session.StartTime, session.EndTime);

            if (overlaps)
            {
                return (false, "Rommet er opptatt. Velg et annet tidspunkt.");
            }

            session.Id = Guid.NewGuid();
            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(StudySession session)
        {
            if (session.EndTime <= session.StartTime)
            {
                return (false, "Sluttidspunkt må være etter starttidspunkt.");
            }

            var overlaps = await _sessionRepository.HasOverlappingSessionAsync(
                session.RoomId, session.StartTime, session.EndTime, session.Id);

            if (overlaps)
            {
                return (false, "Rommet er opptatt. Velg et annet tidspunkt.");
            }

            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                return false;
            }

            _sessionRepository.Delete(session);
            return await _sessionRepository.SaveChangesAsync();
        }
    }
}