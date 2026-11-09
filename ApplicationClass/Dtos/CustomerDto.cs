using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Dtos
{
    public sealed class CustomerGridDto
    { 
        public string Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Phone { get; set; } = default!;

        public string? Address { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = default!;

    } 
    public sealed record CustomerDetailDto(string Id, string Name, string? Phone, string? Address, string? Notes);

    public sealed class CustomerSaveDto
    {
        [Required, MaxLength(150)] public string Name { get; set; } = "";
        [MaxLength(20)] public string? Phone { get; set; }
        [MaxLength(250)] public string? Address { get; set; }
        public string? Notes { get; set; }
    }
}
