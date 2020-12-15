using System;

namespace Endava_Project.Server.Models
{
    public class Wallet
    {  
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool DefaultStatus { get; set; }
    }
}
