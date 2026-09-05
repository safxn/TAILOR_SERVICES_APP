using System;
using System.Collections.Generic;
using System.Text;

namespace DomainLayer.Models
{
    public class Measurement
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int MeasurementTypeId { get; set; }
        public MeasurementType? MeasurementType { get; set; }

        public decimal Value { get; set; }
        public string Unit { get; set; } = "cm";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedById { get; set; } = string.Empty;
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
