using System.ComponentModel.DataAnnotations;

namespace SentryHouseBackend.Dtos
{
    public class ChangePasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        public string newPassword { get; set; } = string.Empty;
    }
}
