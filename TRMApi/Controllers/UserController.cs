using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.Models;
using TRMDataManager.Library.DataAccess;
using TRMApi.Models;
using TRMApi.Data;
using System.Security.Claims;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager
            , IUserData userData, ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        [HttpGet]
        public UserModel GetById()
        {
            // gets userId from the user who is loged in
            // passing userId by method is security issue

            // Old way: string userId = RequestContext.Principal.Identity.GetUserId();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _userData.GetUserById(userId).First();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();

            // Linq
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new {ur.UserId, ur.RoleId, r.Name};

            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                };

                // creates Dictionary with role id and role names for specific user
                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                    .ToDictionary(key => key.RoleId, val => val.Name);

                output.Add(u);
            }
            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
        }

        // passing UserRolePairModel to not expose userId
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddARole(UserRolePairModel pairing)
        {
            //For logging
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            // Don't use string interpolation for logging, user this structure instead.
            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}", 
                loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }

        // passing UserRolePairModel to not expose userId
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]
        public async Task RemoveARole(UserRolePairModel pairing)
        {
            //For logging
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUser = _userData.GetUserById(loggedInUserId).First();

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            // Don't use string interpolation for logging, user this structure instead.
            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}",
                loggedInUserId, user.Id, pairing.RoleName);

            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);
        }

        // REGISTRATION
        public record UserRegistrationModel(string FirstName,
                                            string LastName,
                                            string EmailAddress,
                                            string Password);

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);
                if (existingUser is null) 
                {
                    // DO NOT put password here
                    IdentityUser newUser = new()
                    {
                        Email = user.EmailAddress,
                        EmailConfirmed = true,
                        UserName = user.EmailAddress
                    };

                    IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                    if (result.Succeeded)
                    {
                        existingUser = await _userManager.FindByEmailAsync(user.EmailAddress);

                        // checks if user creation was success and then creates user in our db using authDB user Id
                        if (existingUser is null)
                        {
                            return BadRequest();
                        }

                        UserModel u = new()
                        {
                            Id = existingUser.Id,
                            FirstName = user.FirstName, 
                            LastName = user.LastName,
                            EmailAddress = user.EmailAddress,
                        };

                        _userData.CreateUser(u);
                        return Ok();
                    }
                }
            }

            return BadRequest();
        }
    }
}
