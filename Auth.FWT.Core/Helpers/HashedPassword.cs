using System;

namespace Auth.FWT.Core.Helpers
{
    public class HashedPassword
    {
        public HashedPassword(byte[] salt, byte[] hash)
        {
            Salt = Convert.ToBase64String(salt);
            Hash = Convert.ToBase64String(hash);
        }

        public HashedPassword(string salt, string hash)
        {
            Salt = salt;
            Hash = hash;
        }

        public HashedPassword(string saltedPassword)
        {
            Salt = saltedPassword.Substring(0, 24);
            Hash = saltedPassword.Substring(24);
        }

        private HashedPassword()
        {
        }

        public string Hash { get; private set; }

        public string Salt { get; private set; }

        public byte[] HashToArray()
        {
            return Convert.FromBase64String(Hash);
        }

        public byte[] SaltToArray()
        {
            return Convert.FromBase64String(Salt);
        }

        public string ToSaltedPassword()
        {
            return Salt + Hash;
        }
    }
}
