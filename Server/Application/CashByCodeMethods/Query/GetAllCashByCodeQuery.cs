using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Server.Application.CashByCodeMethods.Query
{
    public class GetAllCashByCodeQuery : IRequest<List<CashByCode>>
    {
        public string UserId { get; set; }
    }

    public class GetCashByCodeHandler : IRequestHandler<GetAllCashByCodeQuery, List<CashByCode>>
    {
        private readonly ApplicationDbContext context;

        public GetCashByCodeHandler(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CashByCode>> Handle(GetAllCashByCodeQuery query, CancellationToken cancellationToken)
        {
            var cashByCodeList = await context.CashByCodeRepo.Where(x => x.SourceUserId == query.UserId).ToListAsync();

            return cashByCodeList;
        }

    }
}
