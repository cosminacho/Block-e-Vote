using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs
{
    [Serializable]
    public class Vote
    {
        public byte[] VoterId { get; set; }
        public byte[] EncryptedVote { get; set; } 
        public byte[] EncryptedKey { get; set; } 
        public byte[] EncryptedIV { get; set; }
    }
}
