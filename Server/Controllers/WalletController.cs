using Microsoft.AspNetCore.Mvc;
using Endava_Project.Server.Models;
using System.Collections.Generic;
using System;
using Endava_Project.Server.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public WalletController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public List<Wallet> GetWallets()
        {
            var userId = userManager.GetUserId(User);
            var wallets = context.Users.Include(e=>e.Wallets).FirstOrDefault(e => e.Id == userId).Wallets;
            return wallets;

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

            var delete_item = context.Wallets.FirstOrDefault(e => e.Id == data);
            context.Wallets.Remove(delete_item);
            context.SaveChanges();
            return Ok();
        }
    }
}
