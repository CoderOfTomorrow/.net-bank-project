using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endava_Project.Server.Models
{
    public class CashByCode
    {
        public Guid Id { get; set; }
        public Guid SourceWalletId { get; set; }
        public string SourceUserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime ExpireTime { get; set; }
        public string GeneratedCode { get; set; }
    }
}
