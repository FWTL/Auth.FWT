using System.ComponentModel.DataAnnotations;

namespace Auth.FWT.Domain.Entities
{
    public class TelegramCode : BaseEntity<string>
    {
        [Required]
        public string CodeHash { get; set; }
    }
}