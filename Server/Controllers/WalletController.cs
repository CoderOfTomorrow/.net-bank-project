using Microsoft.AspNetCore.Mvc;
using Endava_Project.Server.Models;
using System.Collections.Generic;
using System;
using Endava_Project.Server.Data;
using Microsoft.AspNetCore.Identity;
using Wallet = Endava_Project.Server.Models.Wallet;
using MediatR;
using Endava_Project.Server.Application.WalletMethods.Query;
using System.Threading.Tasks;
using Endava_Project.Server.Application.WalletMethods.Command;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMediator mediator;

        public WalletController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this.context = context;
            this.userManager = userManager;
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<List<Wallet>> GetWallets()
        {
            var query = new GetWalletsQuery
            {
                UserId = userManager.GetUserId(User)
            };
            var wallets = await mediator.Send(query);

            return wallets;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Wallet> GetWallet(Guid id)
        {
            var query = new GetWalletQuery
            {
                UserId = userManager.GetUserId(User),
                WalletId = id
            };
            var wallet = await mediator.Send(query);

            return wallet;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromQuery] string currency)
        {

            var command = new CreateWalletCommand
            {
                UserId = userManager.GetUserId(User),
                Currency = currency
            };

            var commandResult = await mediator.Send(command);
            if (!commandResult.IsSuccessful)
                return BadRequest();
            
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteWallet([FromQuery] Guid walletId)
        {
            var command = new DeletWalletCommand
            {
                UserId = userManager.GetUserId(User),
                WalletId = walletId
            };

            var commandResult = await mediator.Send(command);
            if (!commandResult.IsSuccessful)
                return BadRequest();

            return Ok();
        }
    
    }
}
