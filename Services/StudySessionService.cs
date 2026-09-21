using StudyRoom.Models;
using StudyRoom.Repositories;

namespace StudyRoom.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly IStudySessionRepository _sessionRepository;
        private readonly IRepository<Room> _roomRepository;

        private const int MaxDurationHours = 4;
        private const int MaxFutureBookingsPerHost = 3;
        private const int EditLockMinutes = 15;

        public StudySessionService(IStudySessionRepository sessionRepository, IRepository<Room> roomRepository)
        {
            _sessionRepository = sessionRepository;
            _roomRepository = roomRepository;
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
            var validation = await ValidateAsync(session, excludeSessionId: null);
            if (!validation.Success)
            {
                return validation;
            }

            var futureBookings = await _sessionRepository.CountFutureBookingsByHostAsync(session.HostId);
            if (futureBookings >= MaxFutureBookingsPerHost)
            {
                return (false, $"Du kan ikke ha mer enn {MaxFutureBookingsPerHost} kommende bookinger samtidig.");
            }

            session.Id = Guid.NewGuid();
            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(StudySession session)
        {
            var existing = await _sessionRepository.GetByIdAsync(session.Id);
            if (existing == null)
            {
                return (false, "Fant ikke bookingen.");
            }

            if (existing.StartTime <= DateTime.Now.AddMinutes(EditLockMinutes))
            {
                return (false, $"Bookingen kan ikke lenger endres (mindre enn {EditLockMinutes} minutter til start).");
            }

            var validation = await ValidateAsync(session, excludeSessionId: session.Id);
            if (!validation.Success)
            {
                return validation;
            }

            _sessionRepository.Update(session);
            await _sessionRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
            {
                return (false, "Fant ikke bookingen.");
            }

            if (session.StartTime <= DateTime.Now)
            {
                return (false, "Du kan ikke kansellere en booking som allerede har startet eller er avsluttet.");
            }

            _sessionRepository.Delete(session);
            var deleted = await _sessionRepository.SaveChangesAsync();

            return (deleted, deleted ? null : "Kunne ikke slette bookingen.");
        }

        private async Task<(bool Success, string? ErrorMessage)> ValidateAsync(StudySession session, Guid? excludeSessionId)
        {
            if (session.EndTime <= session.StartTime)
            {
                return (false, "Sluttidspunkt må være etter starttidspunkt.");
            }

            if (session.EndTime - session.StartTime > TimeSpan.FromHours(MaxDurationHours))
            {
                return (false, $"En booking kan ikke vare lenger enn {MaxDurationHours} timer.");
            }

            var room = await _roomRepository.GetByIdAsync(session.RoomId);
            if (room == null)
            {
                return (false, "Fant ikke rommet.");
            }

            if (session.MaxParticipants > room.Capacity)
            {
                return (false, $"Rommet har kun plass til {room.Capacity} personer.");
            }

            var roomOverlap = await _sessionRepository.HasOverlappingSessionAsync(
                session.RoomId, session.StartTime, session.EndTime, excludeSessionId);
            if (roomOverlap)
            {
                return (false, "Rommet er opptatt i dette tidsrommet. Velg et annet tidspunkt.");
            }

            var hostOverlap = await _sessionRepository.HasHostOverlappingSessionAsync(
                session.HostId, session.StartTime, session.EndTime, excludeSessionId);
            if (hostOverlap)
            {
                return (false, "Du har allerede en annen booking som overlapper med dette tidspunktet.");
            }

            return (true, null);
        }
    }
}