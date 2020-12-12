using Endava_Project.Server.Application.WalletMethods.Command;
using Endava_Project.Server.Data;
using Endava_Project.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Endava_Project.Tests
{
    public class DeletWalletCommandHandlerTests
    {
        private ApplicationDbContext context;
        public Guid wallet_id = Guid.NewGuid();

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

        [Test] //Test pentru a verifica daca putem sterge un wallet al carui Id exista in BD 
        public async Task DeletWalletSuccessful()
        {
            var sut = new DeletWalletCommandHandler(context);

            var command = new DeletWalletCommand
            {
                UserId = "test_user_id",
                WalletId = wallet_id
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test] //Test pentru a verifica daca putem sterge un wallet al carui Id nu exista in BD 
        public async Task DeletWalletUnsuccessful()
        {
            var sut = new DeletWalletCommandHandler(context);

            var command = new DeletWalletCommand
            {
                UserId = "test_user_id",
                WalletId = new Guid()
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }
    }
}
