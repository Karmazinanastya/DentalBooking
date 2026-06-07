using System.Security.Claims;
using IdentityService.Data;
using IdentityService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    TokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { message = "Невірний email або пароль" });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Невірний email або пароль" });

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.GenerateToken(user, roles);

        return Ok(new LoginResponse(
            token,
            user.Id,
            user.Email!,
            user.FullName,
            roles.FirstOrDefault() ?? "Unknown",
            user.DoctorId,
            user.ClinicId));
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Conflict(new { message = "Користувач з таким email вже існує" });

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            FirstName = request.FirstName,
            LastName = request.LastName ?? string.Empty,
            DoctorId = request.DoctorId,
            ClinicId = request.ClinicId
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return BadRequest(new { errors = createResult.Errors.Select(e => e.Description) });

        var validRoles = new[] { "Admin", "Doctor" };
        var role = validRoles.Contains(request.Role) ? request.Role : "Doctor";
        await userManager.AddToRoleAsync(user, role);

        return Created(string.Empty, new { userId = user.Id, email = user.Email, role });
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = userManager.Users.OrderBy(u => u.Email).ToList();
        var result = new List<object>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            result.Add(new
            {
                u.Id,
                u.Email,
                u.FullName,
                u.FirstName,
                u.LastName,
                u.DoctorId,
                u.ClinicId,
                Roles = roles
            });
        }
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(userId!);
        if (user is null) return NotFound();
        var roles = await userManager.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.FullName, user.DoctorId, user.ClinicId, Roles = roles });
    }

    [HttpDelete("users/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        await userManager.DeleteAsync(user);
        return NoContent();
    }
}

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(
    string AccessToken, Guid UserId, string Email, string FullName, string Role,
    Guid? DoctorId, Guid? ClinicId);
public sealed record RegisterRequest(
    string Email, string Password, string FirstName, string? LastName,
    string Role, Guid? DoctorId, Guid? ClinicId);
