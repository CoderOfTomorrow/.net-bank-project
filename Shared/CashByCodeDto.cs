using System;
using System.Collections.Generic;
using System.Text;

namespace Endava_Project.Shared
{
    public class CashByCodeDto
    {
        public Guid SourceWalletId { get; set; }
        public string SourceUserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime ExpireTime { get; set; }
        public string GeneratedCode { get; set; }
    }
}
