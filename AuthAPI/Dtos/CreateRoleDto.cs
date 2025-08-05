using System.ComponentModel.DataAnnotations;

namespace SentryHouseBackend.Dtos
{
    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name es required")]
        public string RoleName { get; set; } = null!;
    }
}
