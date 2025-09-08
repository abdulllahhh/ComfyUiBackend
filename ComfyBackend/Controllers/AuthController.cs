using infrastructure.Data;
using infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Security.Claims;

namespace ComfyBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager,
                          SignInManager<AppUser> signInManager,
                          JwtService jwtService,
                          ApplicationDbContext context
        ) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly JwtService _jwtService = jwtService;
        private readonly ApplicationDbContext _context = context;
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var user = new AppUser { UserName = req.Username, Email = req.Email, Credits = 10 };
            var result = await _userManager.CreateAsync(user, req.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var token = _jwtService.GenerateToken(user);
            return Ok(new { accessToken = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
            if (!result.Succeeded) return Unauthorized();

            var token = _jwtService.GenerateToken(user);

            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new { accessToken = token, refreshToken = refreshToken.Token });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid refresh token" });

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            var newAccessToken = _jwtService.GenerateToken(user);

            return Ok(new { accessToken = newAccessToken });
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest req)
        {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == req.RefreshToken);
            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Logged out successfully" });
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return Ok(new
            {
                user.Email,
                user.Credits
            });
        }
    }

    public record RefreshRequest(string RefreshToken);
    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Email, string Password);
}

