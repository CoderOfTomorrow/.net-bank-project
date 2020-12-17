using Endava_Project.Server.Application.WalletMethods.Command;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Tests
{
    public class CreateWalletCommandHandlerTests
    {
        private ApplicationDbContext context;

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
                Wallets = new List<Wallet>()
            };

            context.Add(user);

            context.SaveChanges();
        }

        [Test] //Test pentru a verifica daca putem crea un Wallet cu un currency valid 
        public async Task CreateWalletSuccessful()
        {
            var sut = new CreateWalletCommandHandler(context);

            var command = new CreateWalletCommand
            {
                UserId = "test_user_id",
                Currency = "EUR"
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test] //Test pentru a verifica daca putem crea un Wallet cu un currency invalid 
        public async Task CreateWalletUnsuccessful()
        {
            var sut = new CreateWalletCommandHandler(context);

            var command = new CreateWalletCommand
            {
                UserId = "test_user_id",
                Currency = "EURR"
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }
    }
}
