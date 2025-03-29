using System;
using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Product
    {
            [Key]
            public Guid Oid { get; set; } = Guid.NewGuid();

            public required string Name { get; set; }

            public required string Description { get; set; }

            public required decimal Price { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public bool IsDeleted { get; set; } = false;
        
    }
}
