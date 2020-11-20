using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Endava_Project.Server.Application.WalletMethods.Query
{
    public class GetWalletsQuery : IRequest<List<Wallet>>
    {
        public string UserId { get; set; }
    }

    public class GetWalletsQueryHandler : IRequestHandler<GetWalletsQuery, List<Wallet>>
    {
        private readonly ApplicationDbContext context;

        public GetWalletsQueryHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Wallet>> Handle(GetWalletsQuery query, CancellationToken cancellationToken)
        {
            var userWithWallets = await context.Users.Include(x => x.Wallets).FirstOrDefaultAsync(x => x.Id == query.UserId);

            return userWithWallets.Wallets;
        }
    }
}
