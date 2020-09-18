using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class AuthSignedVoteDTO
    {
        public Vote Vote { get; set; }
        public byte[] Signature { get; set; }
    }
}
