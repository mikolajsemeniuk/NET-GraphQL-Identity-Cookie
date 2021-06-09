using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app.Inputs;
using app.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthenticationProperties _authProperties = new AuthenticationProperties
        {
            
        };

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        [HttpGet]
        public string[] check() => new[] 
        { 
            HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            HttpContext.User.FindFirst(ClaimTypes.Email)?.Value 
        };

        private async Task<List<Claim>> GetClaimsList(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }

        [HttpPost("signup")]        
        public async Task<ActionResult> signUpAsync (SignUpInput input)
        {
            if (await _userManager.Users.AnyAsync(user => user.UserName == input.username))
                return BadRequest("Username is taken");

            if (await _userManager.Users.AnyAsync(user => user.Email == input.email))
                return BadRequest("Email is taken");

            var user = new User 
            { 
                Email = input.email,
                UserName = input.username 
            };

            var wasUserSaved = await _userManager.CreateAsync(user, input.password);
            if (!wasUserSaved.Succeeded)
                throw new Exception("something gone wrong try later");
            
            var wereRolesGranted = await _userManager.AddToRolesAsync(user, new[] { "Member" });
            if (!wereRolesGranted.Succeeded)
                throw new Exception("problem occured while granting role");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(new ClaimsIdentity(await GetClaimsList(user), CookieAuthenticationDefaults.AuthenticationScheme)), 
                _authProperties);

            return Ok(new { Authenticated = true });
        }

        [HttpPost("signin")]
        public async Task<ActionResult> signInAsync (SignInInput input)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(
                user => user.Email == input.email);

            if (user == null)
                return BadRequest("Invalid Username");

            var wasUserSignedIn = await _signInManager.CheckPasswordSignInAsync(user, input.password, false);

            if (!wasUserSignedIn.Succeeded)
                return BadRequest("Invalid password");

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(new ClaimsIdentity(await GetClaimsList(user), CookieAuthenticationDefaults.AuthenticationScheme)), 
                _authProperties);

            return Ok(new { Authenticated = true });
        }

        [HttpPost("signout")]
        public async Task<ActionResult> signOutAsync ()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Authenticated = false });
        }
    }
}
