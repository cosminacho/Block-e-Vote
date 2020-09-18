using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class CandidateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Party { get; set; }
    }
}
