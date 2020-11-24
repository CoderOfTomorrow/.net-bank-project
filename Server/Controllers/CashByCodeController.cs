using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Endava_Project.Server.Models;
using Microsoft.AspNetCore.Identity;
using Endava_Project.Shared;
using Endava_Project.Server.Application.CashByCodeMethods.Query;
using Endava_Project.Server.Application.CashByCodeMethods.Command;
using AutoMapper;

namespace Endava_Project.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashByCodeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMediator mediator;

        public CashByCodeController(UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this.userManager = userManager;
            this.mediator = mediator;
        }

        [HttpGet]
        [Route("getall")]
        public async Task<List<CashByCodeDto>> GetAllCashByCode()
        {
            var query = new GetAllCashByCodeQuery
            {
                UserId = userManager.GetUserId(User)
            };

            var cashByCodeList = await mediator.Send(query);
            var list = new List<CashByCodeDto>();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CashByCode, CashByCodeDto>());
            var mapper = new Mapper(config);

            foreach(var data in cashByCodeList)
            {
                var x = mapper.Map<CashByCodeDto>(data);
                list.Add(x);
            }
            

            return list;
        }

        [HttpPost]
        [Route("generate")]
        public async Task<IActionResult> GenerateCashByCode([FromBody] CashByCodeDto data)
        {
            var command = new GenerateCashByCodeCommand
            {
                UserId = userManager.GetUserId(User),
                SourceWalletId = data.SourceWalletId,
                Amount = data.Amount
            };

            var commandResult = await mediator.Send(command);

            if (!commandResult.IsSuccessful)
                return BadRequest();

            return Ok();
        }
    }
}
