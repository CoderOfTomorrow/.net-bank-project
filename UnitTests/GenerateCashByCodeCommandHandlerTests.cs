using Endava_Project.Server.Application.CashByCodeMethods.Command;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Tests
{
    public class GenerateCashByCodeCommandHandlerTests
    {
        private ApplicationDbContext context;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private Guid wallet_id = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            context = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite("Filename=Test.db")
                    .Options, Microsoft.Extensions.Options.Options.Create(new OperationalStoreOptions()));

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var user = new ApplicationUser
            {
                Id = "test_user_id",
                Wallets = new List<Wallet>
                {
                    new Wallet
                    {
                        Id = wallet_id,
                        Amount = 100,
                        Currency = "EUR"
                    }
                }
            };

            context.Add(user);

            context.SaveChanges();
        }

        [Test] 
        public async Task GenerateCashByCodeSuccessful()
        {
            var sut = new GenerateCashByCodeHandler(context,serviceScopeFactory);

            var command = new GenerateCashByCodeCommand
            {
                SourceWalletId = wallet_id,
                UserId = "test_user_id",
                Amount = 100
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public async Task GenerateCashByCodeUnsuccessfulByAmount()
        {
            var sut = new GenerateCashByCodeHandler(context, serviceScopeFactory);

            var command = new GenerateCashByCodeCommand
            {
                SourceWalletId = wallet_id,
                UserId = "test_user_id",
                Amount = 200
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }
    }
}
