using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EVoting.Common.DTOs.VotingDTOs
{
    public class InVoterRegisterDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string CNP { get; set; }

        [Required]
        public string Series { get; set; }

        [Required]
        public string Number { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public byte[] EncryptedPrivateKey { get; set; }
        public byte[] PublicKey { get; set; }

    }
}
