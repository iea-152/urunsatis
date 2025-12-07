using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- BUNU EKLE
using Microsoft.AspNetCore.Http; // <-- BUNU EKLE

namespace urunsatis.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // Bu veritabanında yol tutacak

        public int Stock { get; set; } = 0;

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // --- YENİ EKLENEN KISIM ---
        [NotMapped] // Veritabanına bu kolon eklenmesin dedik
        public IFormFile? ImageUpload { get; set; }
    }
}