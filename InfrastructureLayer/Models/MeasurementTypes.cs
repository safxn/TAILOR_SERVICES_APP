using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DomainLayer.Models
{

        // A customizable measurement field, e.g. "Shoulder", "Sleeve Length", "Kurta Length".
        // Shop owner (Admin) can add new ones without any code changes —
        // each new type automatically becomes selectable as a hotspot on the 3D model.
        public class MeasurementType
        {
            public int Id { get; set; }

            [Required, MaxLength(100)]
            public string Name { get; set; } = string.Empty; // e.g. "Shoulder"

            [MaxLength(20)]
            public string DefaultUnit { get; set; } = "cm"; // cm / inch

            // Optional: maps this measurement type to a hotspot position on the 3D model,
            // so the frontend knows where to place the clickable marker.
            // Stored as "x,y,z" or a named region key like "left_shoulder".
            [MaxLength(50)]
            public string? ModelHotspotKey { get; set; }

            public bool IsActive { get; set; } = true;
        }
     
}
