using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoom.Models;
using StudyRoom.Repositories;

namespace StudyRoom.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRepository<Room> _roomRepository;

        public RoomsController(IRepository<Room> roomRepository)
        {
            _roomRepository = roomRepository;
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
            if (allRooms.Count() >= 10)
            {
                ModelState.AddModelError(string.Empty, "Maks antall rom (10) er allerede nådd.");
            }

            var combinedEquipment = CombineEquipment(selectedEquipment);
            if (!RoomRules.IsValidCombination(room.Building, room.RoomType, combinedEquipment))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig kombinasjon av bygg, romtype og utstyr.");
            }

            if (!ModelState.IsValid)
            {
                PopulateEnumViewBags();
                return View(room);
            }

            room.Id = Guid.NewGuid();
            room.Equipment = combinedEquipment;

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
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
                return NotFound();
            }

            var combinedEquipment = CombineEquipment(selectedEquipment);
            if (!RoomRules.IsValidCombination(room.Building, room.RoomType, combinedEquipment))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig kombinasjon av bygg, romtype og utstyr.");
            }

            if (!ModelState.IsValid)
            {
                PopulateEnumViewBags();
                return View(room);
            }

            room.Equipment = combinedEquipment;

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/{id}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
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
            if (room != null)
            {
                _roomRepository.Delete(room);
                await _roomRepository.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateEnumViewBags()
        {
            ViewBag.Buildings = Enum.GetValues(typeof(Building));
            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType));
            ViewBag.EquipmentOptions = Enum.GetValues(typeof(Equipment))
                .Cast<Equipment>()
                .Where(e => e != Equipment.None);

            ViewBag.RoomTypeRulesJson = JsonSerializer.Serialize(
                RoomRules.AllowedRoomTypes.ToDictionary(
                    kv => ((int)kv.Key).ToString(),
                    kv => kv.Value.Select(v => ((int)v).ToString())));

            ViewBag.EquipmentRulesJson = JsonSerializer.Serialize(
                RoomRules.AllowedEquipment.ToDictionary(
                    kv => ((int)kv.Key).ToString(),
                    kv => kv.Value.Select(v => v.ToString())));
        }

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