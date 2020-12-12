using Endava_Project.Server.Application.TransactionMethods.Command;
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
    public class MakeTransferCommandHandlerTests
    {
        private ApplicationDbContext context;
        public Guid wallet_id_one = Guid.NewGuid();
        public Guid wallet_id_two = Guid.NewGuid();

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
                UserName = "username",
                Wallets = new List<Wallet>
                {
                    new Wallet
                    {
                        Id = wallet_id_one,
                        Amount = 100,
                        Currency = "EUR"
                    },
                    new Wallet
                    {
                        Id = wallet_id_two,
                        Amount = 100,
                        Currency = "USD"
                    }
                }
            };

            context.Add(user);

            context.SaveChanges();
        }

        [Test]
        public async Task MakeTransferSuccesfulByAmount()
        {
            var sut = new MakeTransferCommandHandler(context);

            var command = new MakeTransferCommand
            {
                UserId = "test_user_id",
                Transaction = new Shared.TransferDto
                {
                    SourceId = wallet_id_one,
                    TargetId = wallet_id_two,
                    Username = "username",
                    Amount = 50,
                }
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public async Task MakeTransferUnsuccesfulByAmount()
        {
            var sut = new MakeTransferCommandHandler(context);

            var command = new MakeTransferCommand
            {
                UserId = "test_user_id",
                Transaction = new Shared.TransferDto
                {
                    SourceId = wallet_id_one,
                    TargetId = wallet_id_two,
                    Username = "username",
                    Amount = 150,
                }
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }

        [Test]
        public async Task MakeTransferUnsuccesfulByTargetId()
        {
            var sut = new MakeTransferCommandHandler(context);

            var command = new MakeTransferCommand
            {
                UserId = "test_user_id",
                Transaction = new Shared.TransferDto
                {
                    SourceId = wallet_id_one,
                    TargetId = new Guid(),
                    Username = "username",
                    Amount = 50,
                }
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }

        [Test]
        public async Task MakeTransferUnsuccesfulBySourceId()
        {
            var sut = new MakeTransferCommandHandler(context);

            var command = new MakeTransferCommand
            {
                UserId = "test_user_id",
                Transaction = new Shared.TransferDto
                {
                    SourceId = new Guid(),
                    TargetId = wallet_id_two,
                    Username = "username",
                    Amount = 50,
                }
            };

            var result = await sut.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.IsSuccessful);
        }
    }
}
