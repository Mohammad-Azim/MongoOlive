using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;



using CreateProjectOlive.Dtos;
using CreateProjectOlive.Models;

namespace CreateProjectOlive.Controllers
{
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<ApplicationRole> _roleManager;
        private IConfiguration _configuration;

        public AdminUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration config)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._configuration = config;


        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddAdminDto user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                    // return CreatedAtRoute("api/AdminUser", appUser);
                    return Ok("Admin Created");
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = roleName });
                if (result.Succeeded)
                {
                    return Ok("Role was Added");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("AdminLogin")]
        public async Task<IActionResult> AdminLogIn([FromBody] LoginAdminUserDto LoginData)
        {
            if (ModelState.IsValid)
            {
                //public string Email { get; set; } = null!;
                //public string Password { get; set; } = null!;
                ApplicationUser appUser = await _userManager.FindByEmailAsync(LoginData.Email);
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(appUser, LoginData.Password, false, false);
                    if (result.Succeeded)
                    {
                        var token = GenerateToken();
                        return Ok(token);
                    }
                    else
                    {
                        return BadRequest("Wrong Pass");
                    }
                }
                else
                {
                    return BadRequest("Wrong Email");
                }

            }
            return BadRequest();
        }

        private string GenerateToken()
        {

            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: signIn);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}