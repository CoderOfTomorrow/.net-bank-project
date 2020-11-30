using System.Collections.Generic;

namespace Endava_Project.Shared
{
    public class TransactionHistoryData
    {
        public List<TransactionDto> TransactionsList { get; set; }
        public int TransactionsCount { get; set; }
    }
}
