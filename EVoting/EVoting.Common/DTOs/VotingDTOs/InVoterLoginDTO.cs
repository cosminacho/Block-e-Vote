using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;


namespace EVoting.Common.DTOs.VotingDTOs
{
    public class InVoterLoginDTO
    {
        [Required]
        public string CNP { get; set; }

        [Required]
        public string Password { get; set; }


        [Required]
        public string Series { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
