using MediatR;
using Endava_Project.Server.Models;
using Endava_Project.Server.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Endava_Project.Server.Helpers;

namespace Endava_Project.Server.Application.WalletMethods.Command
{
    public class CreateWalletCommand : IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public string Currency { get; set; }
    }

    public class CreateWalletCommandHandler : IRequestHandler<CreateWalletCommand, CommandResult>
    {
        private readonly ApplicationDbContext context;

        public CreateWalletCommandHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<CommandResult> Handle(CreateWalletCommand command, CancellationToken cancellationToken)
        {
            if (!CurrencyManager.Currencies.Contains(command.Currency))
            {
                return CommandResult.ReturnFailure();
            }

            var user = await context.Users.FindAsync(command.UserId);

            var wallet = new Wallet
            {
                Amount = 0,
                Currency = command.Currency
            };

            user.Wallets.Add(wallet);
            context.SaveChanges();

            return CommandResult.ReturnSuccess();
        }
    }
}
