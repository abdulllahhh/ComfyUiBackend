using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public int Credits { get; set; } = 10; // default free credits
        public ICollection<Payment> Payments { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

    }
}
