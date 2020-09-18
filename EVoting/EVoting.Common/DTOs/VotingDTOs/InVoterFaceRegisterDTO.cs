using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class InVoterFaceRegisterDTO
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public IFormFile Image { get; set; }
    }
}
