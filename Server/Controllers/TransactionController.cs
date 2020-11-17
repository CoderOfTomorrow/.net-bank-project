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
        [Route("transactions")]
        public List<Transaction> GetTransactions()
        {
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();
            transactionsList = context.Transactions.Where(t => idList.Contains(t.SourceWalletId) || idList.Contains(t.DestinationWalletId)).ToList();
            transactionsList = transactionsList.Distinct().ToList();

            return transactionsList;
        }

        [HttpGet]
        [Route("madetransactions")]
        public List<Transaction> GetMadeTransactions()
        {
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();
            transactionsList = context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && t.DestinationUserId != t.SourceUserId).ToList();
            transactionsList = transactionsList.Distinct().ToList();

            return transactionsList;
        }

        [HttpGet]
        [Route("recivedtransactions")]
        public List<Transaction> GetRecivedTransactions()
        {
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();
            transactionsList = context.Transactions.Where(t => idList.Contains(t.DestinationWalletId) && t.DestinationUserId != t.SourceUserId).ToList();
            transactionsList = transactionsList.Distinct().ToList();

            return transactionsList;
        }

        [HttpGet]
        [Route("interntransactions")]
        public List<Transaction> GetInternTransactions()
        {
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();
            transactionsList = context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && idList.Contains(t.DestinationWalletId)).ToList();
            transactionsList = transactionsList.Distinct().ToList();

            return transactionsList;
        }
    }
}
