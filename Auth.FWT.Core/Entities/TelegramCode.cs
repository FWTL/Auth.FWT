using System.ComponentModel.DataAnnotations;
using Auth.FWT.Core.Helpers;

namespace Auth.FWT.Core.Entities
{
    public class TelegramCode : BaseEntity<string>
    {
        public TelegramCode()
        {
        }

        public TelegramCode(string phoneNumber, string hash)
        {
            Id = HashHelper.GetHash(phoneNumber);
            CodeHash = hash;
        }

        [Required]
        public string CodeHash { get; set; }
    }
}
