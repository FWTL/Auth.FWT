using System;
using System.ComponentModel.DataAnnotations;
using Auth.FWT.Core.Helpers;

namespace Auth.FWT.Core.Entities
{
    public class TelegramCode : BaseEntity<string>
    {
        public TelegramCode()
        {
        }

        public TelegramCode(string phoneNumber, string hash, DateTime issuedUTC)
        {
            Id = HashHelper.GetHash(phoneNumber);
            CodeHash = hash;
            IssuedUTC = issuedUTC;
        }

        [Required]
        public string CodeHash { get; set; }

        public DateTime IssuedUTC { get; set; }
    }
}