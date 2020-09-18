using Microsoft.AspNetCore.Identity;
using System;

namespace EVoting.Server.Models
{
    public class User : IdentityUser<Guid>
    {

        public UserDetail UserDetail { get; set; }

    }
}
