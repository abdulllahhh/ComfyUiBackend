using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }   // FK -> User
        public decimal Amount { get; set; }   // real money (USD, EUR, etc.)
        public int CreditsAdded { get; set; } // credits purchased
        public string TransactionId { get; set; } // from payment provider (Stripe, PayPal)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; }
    }
}
