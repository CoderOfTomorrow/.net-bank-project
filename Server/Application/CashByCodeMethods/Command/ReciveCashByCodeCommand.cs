using Endava_Project.Server.Data;
using Endava_Project.Server.Helpers;
using Endava_Project.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Server.Application.CashByCodeMethods.Command
{
    public class ReciveCashByCodeCommand : IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public Guid WalletId { get; set; }
        public string Code { get; set; }
    }

    public class ReciveCashByCodeHandler : IRequestHandler<ReciveCashByCodeCommand, CommandResult>
    {
        private readonly ApplicationDbContext context;

        public ReciveCashByCodeHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<CommandResult> Handle(ReciveCashByCodeCommand command, CancellationToken cancellationToken)
        {
            if(!context.Users.Any(e => e.Id == command.UserId))
                return CommandResult.ReturnFailure();
            
            var user = await context.Users.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == command.UserId);

            if(!user.Wallets.Any(e => e.Id == command.WalletId))
                return CommandResult.ReturnFailure();

            if(!context.CashByCodeRepo.Any(e => e.GeneratedCode == command.Code))
                return CommandResult.ReturnFailure();

            var wallet = await context.Wallets.FirstOrDefaultAsync(e => e.Id == command.WalletId);
            var cashByCode = await context.CashByCodeRepo.FirstOrDefaultAsync(e => e.GeneratedCode == command.Code);

            wallet.Amount += CurrencyManager.CheckCurrency(cashByCode.Amount, cashByCode.Currency, wallet.Currency);
            context.CashByCodeRepo.Remove(cashByCode);
            context.SaveChanges();

            return CommandResult.ReturnSuccess();
        }
    }
}
