using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoom.Models;
using StudyRoom.Repositories;

namespace StudyRoom.Controllers
{
    public class RoomsController : Controller
    {
        // Upper limit on rooms, keeps the scope of the room inventory small
        private const int MaxRooms = 20;

        private readonly IRepository<Room> _roomRepository;
        private readonly IStudySessionRepository _sessionRepository;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(
            IRepository<Room> roomRepository,
            IStudySessionRepository sessionRepository,
            ILogger<RoomsController> logger)
        {
            _roomRepository = roomRepository;
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return View(rooms);
        }

        // GET: Rooms/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning("Room {RoomId} not found (Details)", id);
                return NotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateEnumViewBags();
            return View();
        }

        // POST: Rooms/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Building,RoomType,Capacity")] Room room, Equipment[] selectedEquipment)
        {
            var allRooms = await _roomRepository.GetAllAsync();
            if (allRooms.Count() >= MaxRooms)
            {
                ModelState.AddModelError(string.Empty, $"Maks antall rom ({MaxRooms}) er allerede nådd.");
            }

            var combinedEquipment = CombineEquipment(selectedEquipment);
            if (!RoomRules.IsValidCombination(room.Building, room.RoomType, combinedEquipment))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig kombinasjon av bygg, romtype og utstyr.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid room create attempt for '{RoomName}'", room.Name);
                PopulateEnumViewBags();
                return View(room);
            }

            room.Id = Guid.NewGuid();
            room.Equipment = combinedEquipment;

            try
            {
                await _roomRepository.AddAsync(room);
                await _roomRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating room '{RoomName}'", room.Name);
                ModelState.AddModelError(string.Empty, "Kunne ikke lagre rommet. Prøv igjen.");
                PopulateEnumViewBags();
                return View(room);
            }

            _logger.LogInformation("Room {RoomId} '{RoomName}' created", room.Id, room.Name);
            TempData["SuccessMessage"] = $"Rommet {room.Name} ble opprettet.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning("Room {RoomId} not found (Edit)", id);
                return NotFound();
            }
            PopulateEnumViewBags();
            return View(room);
        }

        // POST: Rooms/Edit/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Building,RoomType,Capacity")] Room room, Equipment[] selectedEquipment)
        {
            if (id != room.Id)
            {
                _logger.LogWarning("Route id {RouteId} does not match room id {RoomId}", id, room.Id);
                return NotFound();
            }

            var combinedEquipment = CombineEquipment(selectedEquipment);
            if (!RoomRules.IsValidCombination(room.Building, room.RoomType, combinedEquipment))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig kombinasjon av bygg, romtype og utstyr.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid room edit attempt for room {RoomId}", room.Id);
                PopulateEnumViewBags();
                return View(room);
            }

            room.Equipment = combinedEquipment;

            try
            {
                _roomRepository.Update(room);
                await _roomRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Also covers the room having been deleted by someone else meanwhile
                _logger.LogError(ex, "Database error while updating room {RoomId}", room.Id);
                ModelState.AddModelError(string.Empty, "Kunne ikke lagre endringene. Rommet kan ha blitt slettet.");
                PopulateEnumViewBags();
                return View(room);
            }

            _logger.LogInformation("Room {RoomId} updated", room.Id);
            TempData["SuccessMessage"] = $"Rommet {room.Name} ble oppdatert.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning("Room {RoomId} not found (Delete)", id);
                return NotFound();
            }

            // Stop early so the admin is not shown a confirm page for a delete that will fail
            if (await HasBookingsAsync(id))
            {
                TempData["ErrorMessage"] = $"Rommet {room.Name} har bookinger og kan ikke slettes.";
                return RedirectToAction(nameof(Index));
            }

            return View(room);
        }

        // POST: Rooms/Delete/{id}
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                _logger.LogWarning("Room {RoomId} not found (DeleteConfirmed)", id);
                return RedirectToAction(nameof(Index));
            }

            // Re-check on the server: a booking may have been made after the confirm page was shown.
            // Rooms use DeleteBehavior.Restrict, so deleting a booked room would throw.
            if (await HasBookingsAsync(id))
            {
                _logger.LogWarning("Blocked delete of room {RoomId}: room has bookings", id);
                TempData["ErrorMessage"] = $"Rommet {room.Name} har bookinger og kan ikke slettes.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _roomRepository.Delete(room);
                await _roomRepository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting room {RoomId}", id);
                TempData["ErrorMessage"] = "Kunne ikke slette rommet. Prøv igjen.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Room {RoomId} '{RoomName}' deleted", id, room.Name);
            TempData["SuccessMessage"] = $"Rommet {room.Name} ble slettet.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> HasBookingsAsync(Guid roomId)
        {
            var sessions = await _sessionRepository.GetSessionsByRoomAsync(roomId);
            return sessions.Any();
        }

        private void PopulateEnumViewBags()
        {
            ViewBag.Buildings = Enum.GetValues(typeof(Building));
            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType));
            ViewBag.EquipmentOptions = Enum.GetValues(typeof(Equipment))
                .Cast<Equipment>()
                .Where(e => e != Equipment.None);

            // Rules serialized to JSON so the view's JavaScript can mirror them (UX only, server stays authoritative)
            ViewBag.RoomTypeRulesJson = JsonSerializer.Serialize(
                RoomRules.AllowedRoomTypes.ToDictionary(
                    kv => ((int)kv.Key).ToString(),
                    kv => kv.Value.Select(v => ((int)v).ToString())));

            ViewBag.EquipmentRulesJson = JsonSerializer.Serialize(
                RoomRules.AllowedEquipment.ToDictionary(
                    kv => ((int)kv.Key).ToString(),
                    kv => kv.Value.Select(v => v.ToString())));
        }

        // Combines the selected checkboxes into one [Flags] value
        private static Equipment CombineEquipment(Equipment[] selected)
        {
            var combined = Equipment.None;
            foreach (var item in selected)
            {
                combined |= item;
            }
            return combined;
        }
    }
}