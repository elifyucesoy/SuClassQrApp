
using Microsoft.AspNetCore.Mvc;
using QRAttendanceSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using SuClassQrApp.DataAccess;
using SuClassQrApp.Business;
using SuClassQrApp.Entities;
using SuClassQrApp.API.DTOs;

namespace SuClassQrApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IAuthService authService, IConfiguration config)
        {
            _context = context;
            _authService = authService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password),
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _authService.GenerateJwtToken(user, _config["Jwt:Secret"]);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !_authService.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _authService.GenerateJwtToken(user, _config["Jwt:Secret"]);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role
            });
        }
    }
}
