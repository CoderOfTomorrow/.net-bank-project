using System;

namespace Endava_Project.Server.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SourceWalletId { get; set; }
        public Guid SourceUserId { get; set; }
        public Guid DestinationWalletId { get; set; }
        public Guid DestinationUserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
    }
}
