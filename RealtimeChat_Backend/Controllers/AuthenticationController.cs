using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealtimeChat.Context;
using RealtimeChat.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealtimeChat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ChatDbContext _context;

        public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration, ChatDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> registerUser([FromBody] RegisterModel model)
        {
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            var nameExists = await _userManager.FindByNameAsync(model.UserName);

            if (emailExists != null)
            {
                return BadRequest("Email already exist");
            }
            if (nameExists != null)
            {
                return BadRequest("UserName is already exist");
            }

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result =await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) 
            {
                return BadRequest("User creation faild, Please check user details and try again");
            }

            return Ok(user);
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName), // ✅ Use this standard claim
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user?.UserName?.ToString()?? ""),
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }


        [HttpGet]
        [Route("GetAllUsers/{loggedInUser}")]
        public async Task<IActionResult> GetAllUsers(string loggedInUser)
        {
            // Get latest message per conversation
            var latestMessages = await _context.Messages
                .Where(m => m.FromUser == loggedInUser || m.UserTo == loggedInUser)
                .GroupBy(m => m.FromUser == loggedInUser ? m.UserTo : m.FromUser)
                .Select(g => g
                    .OrderByDescending(m => m.Created)
                    .FirstOrDefault())
                .ToListAsync();

            // Get all unread messages for logged-in user
            var unreadCounts = await _context.Messages
                .Where(m => m.UserTo == loggedInUser && m.Status == "sent")
                .GroupBy(m => m.FromUser)
                .Select(g => new {
                    FromUser = g.Key,
                    Count = g.Count()
                }).ToListAsync();

            var allUsers = _userManager.Users
                .Where(u => u.UserName != loggedInUser)
                .Select(u => new {
                    u.Id,
                    u.UserName,
                    u.Email
                }).ToList();

            var result = allUsers.Select(u => {
                var msg = latestMessages.FirstOrDefault(m => m.FromUser == u.UserName || m.UserTo == u.UserName);
                var unread = unreadCounts.FirstOrDefault(x => x.FromUser == u.UserName)?.Count ?? 0;

                return new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    LatestMessageTime = msg?.Created,
                    LastMessage = msg?.Message,
                    LastMessageSender = msg?.FromUser,
                    UnreadCount = unread
                };
            }).OrderByDescending(u => u.LatestMessageTime).ToList();

            return Ok(result);
        }





    }
}
