using Identity.Service.Common;
using Identity.Service.Dtos;
using Identity.Service.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Identity.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<UserDto>> Get()
    {
        var users = _userManager.Users
            .ToList()
            .Select(user => user.AsDto());

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
        {
            return NotFound();
        }

        return user.AsDto();
    }

    // /users/{123}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateUserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
        {
            return NotFound();
        }

        user.Email = userDto.Email;
        user.UserName = userDto.Email;
        user.Gil = userDto.Gil;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    // /users/{123}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);

        return NoContent();
    }

}
