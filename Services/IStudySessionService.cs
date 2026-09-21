using StudyRoom.Models;

namespace StudyRoom.Services
{
    public interface IStudySessionService
    {
        Task<IEnumerable<StudySession>> GetAllAsync();
        Task<StudySession?> GetByIdAsync(Guid id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(StudySession session);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(StudySession session);
        Task<bool> DeleteAsync(Guid id);
    }
}