using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.Blockchain
{
    public class Transaction
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public byte[] EncryptedVote { get; set; }
        public byte[] EncryptedKey { get; set; }
        public byte[] EncryptedIV { get; set; }

        public byte[] VoterId { get; set; }
        public byte[] Signature { get; set; }
    }
}
