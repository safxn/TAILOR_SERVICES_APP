using InfrastructureLayer.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace InfrastructureLayer.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = default!;

        public decimal Amount { get; set; } = default!;

        public BillType Type { get; set; } = default!;

        public BillStatus Status { get; set; } = default!;

    }
}
