namespace InfrastructureLayer.Dtos
{
    public sealed class CustomerDto
    {
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public decimal? Chest { get; set; }

        public decimal? Waist { get; set; }

        public decimal? Shoulder { get; set; }

        public decimal? SleeveLength { get; set; }

        public decimal? Neck { get; set; }

        public decimal? Inseam { get; set; }

        public string? FabricPreference { get; set; }

        public string? Notes { get; set; }

    }
}
