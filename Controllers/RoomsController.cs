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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Building,Capacity,Equipment")] Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }

            room.Id = Guid.NewGuid();
            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Building,Capacity,Equipment")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(room);
            }

            _roomRepository.Update(room);
            await _roomRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/{id}
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
    }
}