using Microsoft.AspNetCore.Identity;


namespace DomainLayer.Models;
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

}
