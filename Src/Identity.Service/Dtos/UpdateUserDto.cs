using System.ComponentModel.DataAnnotations;

namespace Identity.Service.Dtos;

public record UpdateUserDto(
        [Required][EmailAddress] string Email,
        [Range(0, 1000000)] decimal Gil
    );