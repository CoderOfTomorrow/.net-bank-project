using System.Collections.Generic;
using System.Linq;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("{typeFilter}/{sortFilter}/{orderFilter}")]
        public List<Transaction> GetTransactions(string typeFilter,string sortFilter,string orderFilter)
        {
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();

            transactionsList = typeFilter switch
            {
                "Made" => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && t.DestinationUserId != t.SourceUserId).ToList(), //for outgoing transactions
                "Recived" => context.Transactions.Where(t => idList.Contains(t.DestinationWalletId) && t.DestinationUserId != t.SourceUserId).ToList(), //for recived transactions
                "Intern" => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && idList.Contains(t.DestinationWalletId)).ToList(), //for transactions betweem our own wallets
                _ => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) || idList.Contains(t.DestinationWalletId)).ToList(), //for all transactions
            };

            transactionsList = sortFilter switch
            {
                "Date" => transactionsList = orderFilter switch
                {
                    "Ascendent" => transactionsList.OrderBy(e => e.Date).ToList(),
                    _ => transactionsList.OrderByDescending(e => e.Date).ToList()
                },


                "Currency" => transactionsList = orderFilter switch
                {
                    "Ascendent" => transactionsList.OrderBy(e => e.Amount).ThenBy(e => e.Currency).ToList(),
                    _ => transactionsList.OrderByDescending(e => e.Amount).ThenByDescending(e => e.Currency).ToList()
                },

                _ => transactionsList.OrderByDescending(e => e.Date).ToList()
                
            };

            transactionsList = transactionsList.Distinct().ToList();

            return transactionsList;
        }
    }
}
