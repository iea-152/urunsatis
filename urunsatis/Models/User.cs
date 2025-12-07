namespace urunsatis.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } // Ad Soyad
        public string Email { get; set; }
        public string Password { get; set; } // Şifre
        public string Role { get; set; } = "User"; // Varsayılan rol
        public string? ProfileImage { get; set; } // Profil resmi (opsiyonel)
    }
}