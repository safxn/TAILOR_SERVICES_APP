using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Text;

namespace DomainLayer.Models;
   public class Customer
   {
    public string Id { get; private init; } = Guid.NewGuid().ToString();

    [Required, MaxLength(150)]
        public string Name { get; private set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; private set; }

        [MaxLength(250)]
        public string? Address { get; private set; }

        public string? Notes { get; private set; }

        // Who registered this customer (admin or employee)
        public string CreatedById { get; private set; } = string.Empty;
        public ApplicationUser? CreatedBy { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Measurement> Measurements { get; private set; } = new List<Measurement>();
        public ICollection<Order> Orders { get; private set; } = new List<Order>();

        #region Factory Method
        public static Customer Create( string name, string phone, string address, string notes, string createdById)
        {
            return new Customer()
            {
                Name = name,
                Phone = phone,
                Address = address,
                Notes = notes,
                CreatedById = createdById
            };
        }

    public void Update(string name, string? phone, string? address, string? notes)
    {
        Name = name; Phone = phone; Address = address; Notes = notes;
    }

    #endregion
}



