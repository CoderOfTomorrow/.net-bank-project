using Microsoft.AspNetCore.Mvc;
using Endava_Project.Server.Models;
using System.Collections.Generic;
using System;
using Endava_Project.Server.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Endava_Project.Shared;
using Wallet = Endava_Project.Server.Models.Wallet;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public WalletController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public List<Wallet> GetWallets()
        {
            var userId = userManager.GetUserId(User);
            var wallets = context.Users.Include(e => e.Wallets).FirstOrDefault(e => e.Id == userId).Wallets;
            return wallets;

        }

        [HttpGet]
        [Route("{id}")]
        public Wallet GetWallet(Guid id)
        {
            var userId = userManager.GetUserId(User);
            var wallet = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId).Wallets.FirstOrDefault(x => x.Id == id);
            return wallet;
        }

        [HttpPost]
        public IActionResult CreateWallet([FromQuery] string currency)
        {

            var new_data = new Wallet
            {
                Amount = 0,
                Currency = currency
            };

            var userId = userManager.GetUserId(User);
            var x = context.Users.Find(userId);
            if (x.Wallets == null)
                x.Wallets = new List<Wallet>();
            x.Wallets.Add(new_data);
            context.SaveChanges();
            return Ok();
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

            if(!context.Users.Any(e => e.UserName == data.Username) || !context.Wallets.Any(e=> e.Id == data.TargetId))
            {
                return BadRequest();
            }

            var source = context.Wallets.FirstOrDefault(e => e.Id == data.SourceId);
            var destinationUser = context.Users.Include(e => e.Wallets).FirstOrDefault(e => e.UserName == data.Username);
            var destination = destinationUser.Wallets.FirstOrDefault(e => e.Id == data.TargetId);

            if(destination == null || source.Amount < data.Amount)
            {
                return BadRequest();
            }

            source.Amount -= data.Amount;
            destination.Amount += data.Amount;

            var transaction = new Transaction
            {
                SourceWalletId = source.Id,
                DestinationWalletId = destination.Id,
                Date = DateTime.Now,
                Amount = data.Amount
            };
            context.Add(transaction);
            context.SaveChanges();

            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteWallet([FromQuery] Guid data)
        {
            //var userId = userManager.GetUserId(User);
            //var user_data = context.Users.Include(e => e.Wallets).FirstOrDefault(e => e.Id == userId);
            //var item_to_remove = user_data.Wallets.FirstOrDefault(e => e.Id == data);
            //user_data.Wallets.Remove(item_to_remove);
            //
            //---Pentru o cautare mai usoara , daca cautam useru si dupa stergem portofelul din lista acestuia ,
            //---el oricum o sa ramana in tabelul cu toate portofelel doar ca nu o sa fie atribuit nici unui user
            //

            var userId = userManager.GetUserId(User);
            var user = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == userId);

            if (!user.Wallets.Any(x => x.Id == data))
            {
                return BadRequest();
            }

            var delete_item = context.Wallets.FirstOrDefault(e => e.Id == data);
            context.Wallets.Remove(delete_item);
            context.SaveChanges();
            return Ok();
        }
    
    }
}
