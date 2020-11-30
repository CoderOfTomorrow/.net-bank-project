using System;
using System.Collections.Generic;
using System.Text;

namespace Endava_Project.Shared
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid SourceWalletId { get; set; }
        public string SourceUserId { get; set; }
        public string SourceUserName { get; set; }
        public Guid DestinationWalletId { get; set; }
        public string DestinationUserId { get; set; }
        public string DestinationUserName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
    }
}
