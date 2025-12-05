

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRAttendanceSystem.DTOs;
using SuClassQrApp.Business;
using SuClassQrApp.DataAccess;
using SuClassQrApp.Entities;
using System.Security.Claims;

namespace SuClassQrApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IQrService _qrService;

        public AttendanceController(AppDbContext context, IQrService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanAttendance(ScanAttendanceDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var evt = await _context.Events
                .FirstOrDefaultAsync(e => e.QrToken == dto.QrToken);

            if (evt == null)
                return BadRequest("Invalid QR code");

            if (!_qrService.IsQrValid(evt.QrToken, evt.QrExpire))
                return BadRequest("QR code has expired");

            // Daha önce kayıt var mı kontrol et
            var existingAttendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.EventId == evt.Id && a.UserId == userId);

            if (existingAttendance != null)
                return BadRequest("Already marked attendance for this event");

            var attendance = new Attendance
            {
                EventId = evt.Id,
                UserId = userId,
                ScanTime = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Attendance recorded successfully" });
        }

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult> GetEventAttendance(int eventId)
        {
            var attendances = await _context.Attendances
                .Where(a => a.EventId == eventId)
                .Include(a => a.User)
                .Select(a => new
                {
                    id = a.Id,
                    userName = a.User.Name,
                    userEmail = a.User.Email,
                    scanTime = a.ScanTime
                })
                .ToListAsync();

            return Ok(attendances);
        }

        [HttpGet("user/me")]
        public async Task<ActionResult> GetMyAttendance()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId)
                .Include(a => a.Event)
                .Select(a => new
                {
                    id = a.Id,
                    eventTitle = a.Event.Title,
                    eventDate = a.Event.Date,
                    scanTime = a.ScanTime
                })
                .ToListAsync();

            return Ok(attendances);
        }
    }
}

