

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRAttendanceSystem.DTOs;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SuClassQrApp.DataAccess;
using SuClassQrApp.Business;
using SuClassQrApp.Entities;
using API.DTOs;

namespace SuClassQrApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IQrService _qrService;

        public EventsController(AppDbContext context, IQrService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventDto>>> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    QrToken = e.QrToken,
                    QrExpire = e.QrExpire,
                    Status = e.Status,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<EventDto>> CreateEvent(CreateEventDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var newEvent = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                Date = dto.Date,
                QrToken = _qrService.GenerateQrToken(),
                QrExpire = _qrService.GetQrExpiry(),
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new EventDto
            {
                Id = newEvent.Id,
                Title = newEvent.Title,
                Description = newEvent.Description,
                Date = newEvent.Date,
                QrToken = newEvent.QrToken,
                QrExpire = newEvent.QrExpire,
                Status = newEvent.Status,
                CreatedAt = newEvent.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt == null)
                return NotFound();

            return Ok(new EventDto
            {
                Id = evt.Id,
                Title = evt.Title,
                Description = evt.Description,
                Date = evt.Date,
                QrToken = evt.QrToken,
                QrExpire = evt.QrExpire,
                Status = evt.Status,
                CreatedAt = evt.CreatedAt
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateEvent(int id, CreateEventDto dto)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt == null)
                return NotFound();

            evt.Title = dto.Title;
            evt.Description = dto.Description;
            evt.Date = dto.Date;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt == null)
                return NotFound();

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/qr")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> GetEventQr(int id)
        {
            var evt = await _context.Events.FindAsync(id);

            if (evt == null)
                return NotFound();

            // QR süresini yenile
            evt.QrToken = _qrService.GenerateQrToken();
            evt.QrExpire = _qrService.GetQrExpiry();
            await _context.SaveChangesAsync();

            return Ok(new { qrToken = evt.QrToken, qrExpire = evt.QrExpire });
        }
    }
}
