using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Endava_Project.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Wallet> Wallets { get; set; }
    }
}
