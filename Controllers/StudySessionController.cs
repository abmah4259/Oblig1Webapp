using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using StudyRoom.Models;
using StudyRoom.Repositories;
using StudyRoom.Services;

namespace StudyRoom.Controllers
{
    public class StudySessionController : Controller
    {
        private readonly IStudySessionService _sessionService;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Student> _studentRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<StudySessionController> _logger;

        public StudySessionController(
            IStudySessionService sessionService,
            IRepository<Room> roomRepository,
            IRepository<Student> studentRepository,
            UserManager<IdentityUser> userManager,
            ILogger<StudySessionController> logger)
        {
            _sessionService = sessionService;
            _roomRepository = roomRepository;
            _studentRepository = studentRepository;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: StudySession
        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionService.GetAllAsync();
            var currentStudent = await GetCurrentStudentAsync();
            ViewBag.CurrentStudentId = currentStudent?.Id;
            return View(sessions);
        }

        // GET: StudySession/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }

        // GET: StudySession/Create?roomId={id}
        // roomId is optional; when coming from the room list, that room is preselected
        [Authorize]
        public async Task<IActionResult> Create(Guid? roomId)
        {
            await PopulateRoomsViewBagAsync(roomId);

            var session = new StudySession();
            if (roomId.HasValue)
            {
                session.RoomId = roomId.Value;
            }
            return View(session);
        }

        // POST: StudySession/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,Subject,Topic,StartTime,EndTime,MaxParticipants")] StudySession session)
        {
            var currentStudent = await GetCurrentStudentAsync();
            if (currentStudent == null)
            {
                return Forbid();
            }

            session.HostId = currentStudent.Id;

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning("ModelState-feil for felt '{Field}': {Error}", entry.Key, error.ErrorMessage);
                    }
                }
                await PopulateRoomsViewBagAsync(session.RoomId);
                return View(session);
            }

            var (success, errorMessage) = await _sessionService.CreateAsync(session);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Kunne ikke opprette booking.");
                await PopulateRoomsViewBagAsync(session.RoomId);
                return View(session);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: StudySession/Edit/{id}
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            var currentStudent = await GetCurrentStudentAsync();
            if (currentStudent == null || session.HostId != currentStudent.Id)
            {
                return Forbid();
            }

            await PopulateRoomsViewBagAsync(session.RoomId);
            return View(session);
        }

        // POST: StudySession/Edit/{id}
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RoomId,Subject,Topic,StartTime,EndTime,MaxParticipants")] StudySession session)
        {
            if (id != session.Id)
            {
                return NotFound();
            }

            var currentStudent = await GetCurrentStudentAsync();
            if (currentStudent == null)
            {
                return Forbid();
            }

            var existing = await _sessionService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (existing.HostId != currentStudent.Id)
            {
                return Forbid();
            }

            session.HostId = currentStudent.Id;

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        _logger.LogWarning("ModelState-feil for felt '{Field}': {Error}", entry.Key, error.ErrorMessage);
                    }
                }
                await PopulateRoomsViewBagAsync(session.RoomId);
                return View(session);
            }

            var (success, errorMessage) = await _sessionService.UpdateAsync(session);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Kunne ikke oppdatere booking.");
                await PopulateRoomsViewBagAsync(session.RoomId);
                return View(session);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: StudySession/Delete/{id}
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            var currentStudent = await GetCurrentStudentAsync();
            if (currentStudent == null || session.HostId != currentStudent.Id)
            {
                return Forbid();
            }

            return View(session);
        }

        // POST: StudySession/Delete/{id}
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var session = await _sessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            var currentStudent = await GetCurrentStudentAsync();
            if (currentStudent == null || session.HostId != currentStudent.Id)
            {
                return Forbid();
            }

            var (success, errorMessage) = await _sessionService.DeleteAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return null;
            }
            return await _studentRepository.FindAsync(s => s.UserId == userId);
        }

        private async Task PopulateRoomsViewBagAsync(Guid? selectedRoomId = null)
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomOptions = rooms.Select(r => new
            {
                r.Id,
                Display = $"{r.Name} ({r.Building}, {r.RoomType}, kapasitet {r.Capacity})"
            });
            ViewBag.Rooms = new SelectList(roomOptions, "Id", "Display", selectedRoomId);
        }
    }
}