using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class InVoterFaceLoginDTO
    {
        public IFormFile Image { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
