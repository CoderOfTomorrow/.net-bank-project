using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using Endava_Project.Shared;
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
        [Route("{typeFilter}/{sortFilter}/{orderFilter}/{itemsPerPage}/{pageNumber}")]
        public TransactionHistoryData GetTransactions(string typeFilter,string sortFilter,string orderFilter, int itemsPerPage, int pageNumber)
        {
            var transactionData = new TransactionHistoryData { 
                TransactionsList = new List<TransactionDto>()
            };
            var transactionsList = new List<Transaction>();
            var userId = userManager.GetUserId(User);
            var idList = context.Users.Include(e => e.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.Select(e => e.Id).ToList();

            transactionsList = typeFilter switch
            {
                "Made" => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && t.DestinationUserId != t.SourceUserId).ToList(), //for outgoing transactions
                "Recived" => context.Transactions.Where(t => idList.Contains(t.DestinationWalletId) && t.DestinationUserId != t.SourceUserId).ToList(), //for recived transactions
                "Intern" => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) && idList.Contains(t.DestinationWalletId)).Distinct().ToList(), //for transactions betweem our own wallets
                _ => context.Transactions.Where(t => idList.Contains(t.SourceWalletId) || idList.Contains(t.DestinationWalletId)).Distinct().ToList(), //for all transactions
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
                    "Ascendent" => transactionsList.OrderBy(e => e.Currency).ThenBy(e => e.Amount).ToList(),
                    _ => transactionsList.OrderByDescending(e => e.Currency).ThenByDescending(e => e.Amount).ToList()
                },

                "Target" => transactionsList = typeFilter switch
                {
                    "Made" => transactionsList = orderFilter switch
                    {
                        "Ascendent" => transactionsList.OrderBy(e => e.DestinationUserName).ToList(),
                        _ => transactionsList.OrderByDescending(e => e.DestinationUserName).ToList()
                    },
                    "Recived" => transactionsList = orderFilter switch
                    {
                        "Ascendent" => transactionsList.OrderBy(e => e.SourceUserName).ToList(),
                        _ => transactionsList.OrderByDescending(e => e.SourceUserName).ToList()
                    },
                    _ => transactionsList.ToList()
                },

                _ => transactionsList.OrderByDescending(e => e.Date).ToList()
                
            };
            transactionData.TransactionsCount = transactionsList.Count;
            transactionsList = transactionsList.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Transaction, TransactionDto>());
            var mapper = new Mapper(config);

            foreach (var transaction in transactionsList)
            {
                var t = mapper.Map<TransactionDto>(transaction);
                transactionData.TransactionsList.Add(t);
            }

            return transactionData;
        }

        [HttpPost]
        [Route("transfer")]
        public IActionResult MakeTransfer([FromBody] TransferDto data)
        {
            var userId = userManager.GetUserId(User);
            var user = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId);
            if (!user.Wallets.Any(x => x.Id == data.SourceId))
            {
                return BadRequest();
            }

            if (!context.Users.Any(e => e.UserName == data.Username) || !context.Wallets.Any(e => e.Id == data.TargetId))
            {
                return BadRequest();
            }

            var source = context.Wallets.FirstOrDefault(e => e.Id == data.SourceId);
            var destinationUser = context.Users.Include(e => e.Wallets).FirstOrDefault(e => e.UserName == data.Username);
            var destination = destinationUser.Wallets.FirstOrDefault(e => e.Id == data.TargetId);

            if (destination == null || source.Amount < data.Amount)
            {
                return BadRequest();
            }

            source.Amount -= data.Amount;
            decimal destinationAmount = CurrencyManager.CheckCurrency(data.Amount, source.Currency, destination.Currency);
            destination.Amount += destinationAmount;

            var transaction = new Transaction
            {
                SourceWalletId = source.Id,
                SourceUserId = Guid.Parse(userId),
                SourceUserName = user.UserName,
                DestinationWalletId = destination.Id,
                DestinationUserId = Guid.Parse(destinationUser.Id),
                DestinationUserName = destinationUser.UserName,
                Date = DateTime.Now,
                Amount = data.Amount,
                Currency = source.Currency
            };
            context.Add(transaction);
            context.SaveChanges();

            return Ok();
        }
    }
}
