using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Endava_Project.Server.Data;
using Microsoft.EntityFrameworkCore;
using Endava_Project.Server.Helpers;
using System.Threading;

namespace Endava_Project.Server.Application.WalletMethods.Command
{
    public class DeletWalletCommand : IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public Guid WalletId { get; set; }
    }

    public class DeletWalletCommandHandler : IRequestHandler<DeletWalletCommand, CommandResult>
    {
        private readonly ApplicationDbContext context;

        public DeletWalletCommandHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<CommandResult> Handle(DeletWalletCommand command, CancellationToken cancellationToken)
        {
            
            var user = context.Users.Include(x => x.Wallets).FirstOrDefault(x => x.Id == command.UserId);

            if (!user.Wallets.Any(x => x.Id == command.WalletId))
            {
                return CommandResult.ReturnFailure();
            }

            var wallet = context.Wallets.FirstOrDefault(e => e.Id == command.WalletId);
            context.Wallets.Remove(wallet);
            context.SaveChanges();

            return CommandResult.ReturnSuccess();
        }
    }
}
