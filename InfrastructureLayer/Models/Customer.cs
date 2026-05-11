using System;
using System.Collections.Generic;
using System.Text;

namespace DomainLayer.Models
{
    public class Customer
    {
        public int Id { get; set; }

        // Basic Details
        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        // Address
        public string? Address { get; set; }

        // Tailor Specific
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Measurements
        public decimal? Chest { get; set; }

        public decimal? Waist { get; set; }

        public decimal? Shoulder { get; set; }

        public decimal? SleeveLength { get; set; }

        public decimal? Neck { get; set; }

        public decimal? Inseam { get; set; }

        // Preferences / Notes
        public string? FabricPreference { get; set; }

        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}

