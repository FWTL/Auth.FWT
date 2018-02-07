using System.ComponentModel.DataAnnotations;

namespace Auth.FWT.Domain.Entities
{
    public class TelegramCode : BaseEntity<string>
    {
        public TelegramCode()
        {
        }

        public TelegramCode(string phoneNumber, string hash)
        {
            Id = phoneNumber;
            CodeHash = hash;
        }

        [Required]
        public string CodeHash { get; set; }
    }
}