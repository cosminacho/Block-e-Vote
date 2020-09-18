using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class UserSignedVoteDTO
    {
        public Vote Vote { get; set; }
        public byte[] Signature { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
    }
}
