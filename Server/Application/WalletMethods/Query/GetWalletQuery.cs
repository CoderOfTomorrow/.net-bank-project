using MediatR;
using Endava_Project.Server.Models;
using System;
using Endava_Project.Server.Data;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Endava_Project.Server.Application.WalletMethods.Query
{
    public class GetWalletQuery : IRequest<Wallet>
    {
        public string UserId { get; set; }
        public Guid WalletId { get; set; }
    }

    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, Wallet>
    {
        private readonly ApplicationDbContext context;

        public GetWalletQueryHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Wallet> Handle(GetWalletQuery query, CancellationToken cancellationToken)
        {
            //var userWallets = await context.Users.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == query.UserId);
            //var Wallet = userWallets.Wallets.FirstOrDefault(x => x.Id == query.WalletId);

            var Wallet = await context.Users.Where(u => u.Id == query.UserId).SelectMany(u => u.Wallets).Where(w => w.Id == query.WalletId).FirstOrDefaultAsync();

            return Wallet;
        }
    }
}
