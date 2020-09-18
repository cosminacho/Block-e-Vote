using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class OutVoterLoginDTO
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public byte[] CAuthPublicKey { get; set; }
        public byte[] EncryptedPrivateKey { get; set; }
        public byte[] PublicKey { get; set; }

    }
}
