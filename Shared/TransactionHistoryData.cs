using System;
using System.Collections.Generic;
using System.Text;

namespace Endava_Project.Shared
{
    public class TransactionHistoryData
    {
        public List<TransactionDto> TransactionsList { get; set; }
        public int TransactionsCount { get; set; }
    }
}
