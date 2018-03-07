using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLSharp.Core.MTProto.Crypto
{
    public class GetFutureSaltsResponse
    {
        private int now;

        private ulong requestId;

        private SaltCollection salts;

        public GetFutureSaltsResponse(ulong requestId, int now)
        {
            this.requestId = requestId;
            this.now = now;
        }

        public int Now
        {
            get { return now; }
        }

        public ulong RequestId
        {
            get { return requestId; }
        }

        public SaltCollection Salts
        {
            get { return salts; }
        }

        public void AddSalt(Salt salt)
        {
            salts.Add(salt);
        }
    }

    public class Salt : IComparable<Salt>
    {
        private ulong salt;

        private int validSince;

        private int validUntil;

        public Salt(int validSince, int validUntil, ulong salt)
        {
            this.validSince = validSince;
            this.validUntil = validUntil;
            this.salt = salt;
        }

        public int ValidSince
        {
            get { return validSince; }
        }

        public int ValidUntil
        {
            get { return validUntil; }
        }

        public ulong Value
        {
            get { return salt; }
        }

        public int CompareTo(Salt other)
        {
            return validUntil.CompareTo(other.validSince);
        }
    }

    public class SaltCollection
    {
        private SortedSet<Salt> salts;

        public int Count
        {
            get
            {
                return salts.Count;
            }
        }

        public void Add(Salt salt)
        {
            salts.Add(salt);
        }
    }
}
