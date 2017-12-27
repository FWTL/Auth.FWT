﻿using System;
using System.ComponentModel.DataAnnotations;
using Auth.FWT.Core.DomainModels.Identity;

namespace Auth.FWT.Core.DomainModels
{
    public class TelegramSession : BaseEntity<int>
    {
        public byte[] Session { get; set; }
        public int UserId { get; set; }

        [Required]
        public virtual User User { get; set; }

        public DateTime ExpireDateUtc { get; set; }
    }
}