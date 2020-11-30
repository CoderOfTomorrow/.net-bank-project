using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Server.Application.TransactionMethods.Query
{
    public class GetTransactionsQuery : IRequest<List<Transaction>>
    {
        public string UserId { get; set; }
        public string TypeFilter { get; set; }
        public string SortFilter { get; set; }
        public string OrderFilter { get; set; }
    }

    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionsQuery, List<Transaction>>
    {
        private readonly ApplicationDbContext context;

        public GetTransactionQueryHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Transaction>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken)
        {

            
            var transactionsList = new List<Transaction>();

            var userWithWallets = await context.Users.Include(e => e.Wallets).FirstOrDefaultAsync(x => x.Id == query.UserId);
            

            transactionsList = query.TypeFilter switch
            {
                "Made" => context.Transactions.Where(t => query.UserId.Contains(t.SourceUserId) && t.DestinationUserId != t.SourceUserId).ToList(), //for outgoing transactions
                "Recived" => context.Transactions.Where(t => query.UserId.Contains(t.DestinationUserId) && t.DestinationUserId != t.SourceUserId).ToList(), //for recived transactions
                "Intern" => context.Transactions.Where(t => query.UserId.Contains(t.SourceUserId) && query.UserId.Contains(t.DestinationUserId)).Distinct().ToList(), //for transactions betweem our own wallets
                _ => context.Transactions.Where(t => query.UserId.Contains(t.SourceUserId) || query.UserId.Contains(t.DestinationUserId)).Distinct().ToList(), //for all transactions
            };

            transactionsList = query.SortFilter switch
            {
                "Date" => transactionsList = query.OrderFilter switch
                {
                    "Ascendent" => transactionsList.OrderBy(e => e.Date).ToList(),
                    _ => transactionsList.OrderByDescending(e => e.Date).ToList()
                },


                "Currency" => transactionsList = query.OrderFilter switch
                {
                    "Ascendent" => transactionsList.OrderBy(e => e.Currency).ThenBy(e => e.Amount).ToList(),
                    _ => transactionsList.OrderByDescending(e => e.Currency).ThenByDescending(e => e.Amount).ToList()
                },

                "Target" => transactionsList = query.TypeFilter switch
                {
                    "Made" => transactionsList = query.OrderFilter switch
                    {
                        "Ascendent" => transactionsList.OrderBy(e => e.DestinationUserName).ToList(),
                        _ => transactionsList.OrderByDescending(e => e.DestinationUserName).ToList()
                    },
                    "Recived" => transactionsList = query.OrderFilter switch
                    {
                        "Ascendent" => transactionsList.OrderBy(e => e.SourceUserName).ToList(),
                        _ => transactionsList.OrderByDescending(e => e.SourceUserName).ToList()
                    },
                    _ => transactionsList.ToList()
                },

                _ => transactionsList.OrderByDescending(e => e.Date).ToList()

            };

            return transactionsList;
        }
    }
}
