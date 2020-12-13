using System;

namespace Endava_Project.Shared
{
    public class WalletDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool DefaultStatus { get; set; }
    }
}
