using DomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DomainLayer.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required, MaxLength(100)]
        public string MaterialType { get; set; } = string.Empty; // e.g. "Cotton", "Linen"

        public decimal Quantity { get; set; } // e.g. meters of fabric
        public decimal Cost { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Employee/tailor this order is assigned to
        public string? AssignedToId { get; set; }
        public ApplicationUser? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
    }
}
