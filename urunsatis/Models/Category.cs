using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Mvc; // Buna gerek yok
// using Microsoft.EntityFrameworkCore; // Buna gerek yok

namespace urunsatis.Models
{
    public class Category
    {
        [Key] // Id'nin anahtar olduğunu belirtelim
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur")] // Hata mesajı ekleyelim
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        // SORUN ÇIKARAN KISIM BURASI OLABİLİR:
        // Listeyi "nullable" (?) yapalım ki boş gelirse hata vermesin
        public List<Product>? Products { get; set; }
    }
}