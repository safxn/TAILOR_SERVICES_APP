namespace DomainLayer.Dtos.Users;


public sealed record UserGridDto
{
    public required string Id { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public string? PhoneNumber { get; init; }
}
public sealed class UserDetailDto : InputVersionField
{
    public required string Id { get; init; }

    public required string FullName { get; init; } 

    public required string Email { get; init; } 

    public string? PhoneNumber { get; init; }

}

public sealed record UserCreateDto
{
    public required string FullName { get; init; } 

    public required string Email { get; init; } 

    public  string? PhoneNumber { get; init; }

    public required string Password { get; init; } 
}

public sealed record UserUpdateDto
{
    public required string Id { get; set; } 

    public required string FullName { get; set; } 

    public required string Email { get; set; } 

    public  string? PhoneNumber { get; set; }

    public string? Password { get; set; }  
}