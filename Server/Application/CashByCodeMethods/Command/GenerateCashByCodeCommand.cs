using Endava_Project.Server.Data;
using Endava_Project.Server.Helpers;
using Endava_Project.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Server.Application.CashByCodeMethods.Command
{
    public class GenerateCashByCodeCommand : IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public Guid SourceWalletId { get; set; }
        public decimal Amount { get; set; }
    }

    public class GenerateCashByCodeHandler : IRequestHandler<GenerateCashByCodeCommand, CommandResult>
    {
        private readonly ApplicationDbContext context;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public GenerateCashByCodeHandler(ApplicationDbContext context, IServiceScopeFactory serviceScopeFactory)
        {
            this.context = context;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<CommandResult> Handle(GenerateCashByCodeCommand command , CancellationToken cancellationToken)
        {
            int codeLenght = 6;

            var user = await context.Users.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == command.UserId);

            if (!user.Wallets.Any(x => x.Id == command.SourceWalletId))
                return CommandResult.ReturnFailure();

            var sourceWallet = context.Wallets.FirstOrDefault(e => e.Id == command.SourceWalletId);

            if(sourceWallet.Amount == 0 || sourceWallet.Amount<command.Amount)
                return CommandResult.ReturnFailure();

            sourceWallet.Amount -= command.Amount;

            var cashByCode = new CashByCode
            {
                SourceWalletId = command.SourceWalletId,
                SourceUserId = command.UserId,
                Amount = command.Amount,
                Currency = sourceWallet.Currency,
                ExpireTime = DateTime.Now.AddMinutes(60),
                GeneratedCode = StringGenerator.Generator(codeLenght)
            };
            context.Add(cashByCode);
            context.SaveChanges();

            _ = Task.Run(() => LifeSpan(cashByCode.Id, serviceScopeFactory));

            return CommandResult.ReturnSuccess();
        }


        private async Task LifeSpan(Guid Id,IServiceScopeFactory serviceScopeFactory)
        {
            await Task.Delay(3600000);
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var itemToRemove = dbContext.CashByCodeRepo.FirstOrDefault(e => e.Id == Id);
            dbContext.CashByCodeRepo.Remove(itemToRemove);
            await dbContext.SaveChangesAsync();
        }
    }
}
