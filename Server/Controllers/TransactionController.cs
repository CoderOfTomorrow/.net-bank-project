using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Endava_Project.Server.Application.TransactionMethods.Command;
using Endava_Project.Server.Application.TransactionMethods.Query;
using Endava_Project.Server.Models;
using Endava_Project.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMediator mediator;

        public TransactionController(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this.userManager = userManager;
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("{typeFilter}/{sortFilter}/{orderFilter}/{itemsPerPage}/{pageNumber}")]
        public TransactionHistoryData GetTransactions(string typeFilter,string sortFilter,string orderFilter, int itemsPerPage, int pageNumber)
        {
            var transactionData = new TransactionHistoryData
            {
                TransactionsList = new List<TransactionDto>()
            };

            var query = new GetTransactionsQuery
            {
                UserId = userManager.GetUserId(User),
                TypeFilter = typeFilter,
                SortFilter = sortFilter,
                OrderFilter = orderFilter
            };

            var transactionsList = mediator.Send(query).GetAwaiter().GetResult();

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
        public async Task<IActionResult> MakeTransfer([FromBody] TransferDto data)
        {
            var userId = userManager.GetUserId(User);

            var command = new MakeTransferCommand
            {
                UserId = userId,
                Transaction = data
            };

            var commandResult = await mediator.Send(command);

            if (!commandResult.IsSuccessful)
                return BadRequest();

            return Ok();
        }
    }
}
