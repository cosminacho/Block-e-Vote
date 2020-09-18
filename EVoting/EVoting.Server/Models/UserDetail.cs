using System;
using System.ComponentModel.DataAnnotations;

namespace EVoting.Server.Models
{
    public class UserDetail
    {
        [Key]
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string CNP { get; set; }
        public string Series { get; set; }
        public string Number { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public byte[] EncryptedPrivateKey { get; set; }
        public byte[] PublicKey { get; set; }

        public bool HasVoted { get; set; }
    }
}
