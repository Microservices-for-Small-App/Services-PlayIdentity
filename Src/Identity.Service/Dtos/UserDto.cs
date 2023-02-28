namespace Identity.Service.Dtos;

public record UserDto(
        Guid Id,
        string Username,
        string Email,
        decimal Gil,
        DateTimeOffset CreatedDate
    );