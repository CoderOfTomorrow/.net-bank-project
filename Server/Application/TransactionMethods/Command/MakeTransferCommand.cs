using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Endava_Project.Server.Helpers;
using Endava_Project.Server.Models;
using Endava_Project.Server.Data;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Endava_Project.Shared;

namespace Endava_Project.Server.Application.TransactionMethods.Command
{
    public class MakeTransferCommand : IRequest<CommandResult>
    {
        public string UserId { get; set; }
        public TransferDto Transaction { get; set; }
    }

    public class MakeTransferCommandHandler : IRequestHandler<MakeTransferCommand, CommandResult>
    {
        private readonly ApplicationDbContext context;
        public MakeTransferCommandHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public  async Task<CommandResult> Handle(MakeTransferCommand command, CancellationToken cancellationToken)
        {
            var user = await context.Users.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == command.UserId);
            if (!user.Wallets.Any(x => x.Id == command.Transaction.SourceId))
            {
                return CommandResult.ReturnFailure();
            }

            if (!context.Users.Any(e => e.UserName == command.Transaction.Username) || !context.Wallets.Any(e => e.Id == command.Transaction.TargetId))
            {
                return CommandResult.ReturnFailure();
            }

            var source = context.Wallets.FirstOrDefault(e => e.Id == command.Transaction.SourceId);
            var destinationUser = context.Users.Include(e => e.Wallets).FirstOrDefault(e => e.UserName == command.Transaction.Username);
            var destination = destinationUser.Wallets.FirstOrDefault(e => e.Id == command.Transaction.TargetId);

            if (destination == null || source.Amount < command.Transaction.Amount)
            {
                return CommandResult.ReturnFailure();
            }

            source.Amount -= command.Transaction.Amount;
            decimal destinationAmount = CurrencyManager.CheckCurrency(command.Transaction.Amount, source.Currency, destination.Currency);
            destination.Amount += destinationAmount;

            var transaction = new Transaction
            {
                SourceWalletId = source.Id,
                SourceUserId = command.UserId,
                SourceUserName = user.UserName,
                DestinationWalletId = destination.Id,
                DestinationUserId = destinationUser.Id,
                DestinationUserName = destinationUser.UserName,
                Date = DateTime.Now,
                Amount = command.Transaction.Amount,
                Currency = source.Currency
            };
            context.Add(transaction);
            context.SaveChanges();

            return CommandResult.ReturnSuccess();
        }
    }
}
