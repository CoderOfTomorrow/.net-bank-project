using System;
using System.Collections.Generic;
using System.Text;

namespace Endava_Project.Shared
{
    public class TransferDto
    {
        public Guid SourceId { get; set; }
        public Guid TargetId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
