using Api.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Controllers.V1;

[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        return "AuthController - Accessed in: " + DateTime.UtcNow.ToLongDateString();
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] UserDTO userDto)
    {
        var user = new IdentityUser
        {
            UserName = userDto.Name,
            Email = userDto.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, userDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok(GenerateToken(userDto));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserDTO userDto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            userDto.Email,
            userDto.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return Ok(GenerateToken(userDto));
        }

        ModelState.AddModelError(string.Empty, "Invalid login...");
        return BadRequest(ModelState);
    }

    private UserToken GenerateToken(UserDTO userDto)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, userDto.Email),
            new Claim("App", "CatalogoApi"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expire = _config["TokenConfiguration:ExpireHours"];
        var expiration = DateTime.UtcNow.AddHours(double.Parse(expire));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _config["TokenConfiguration:Issuer"],
            audience: _config["TokenConfiguration:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new UserToken()
        {
            Authenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Message = "Token JWT OK"
        };
    }
}
