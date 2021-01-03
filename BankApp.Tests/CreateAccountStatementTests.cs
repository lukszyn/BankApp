using BankApp.BusinessLayer;
using BankApp.BusinessLayer.Models;
using BankApp.DataLayer;
using BankApp.DataLayer.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BankApp.Tests
{
    internal class BankAppDbContextMock : DbContext, IBankAppDbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("BankAppDb");
        }
    }

    public class Tests
    {
        [Test]
        public void CreateAccountStatement_NoTransfers_ReturnsZeros()
        {
            var accountStatementPattern = new AccountStatement()
            {
                OutgoingTransfersStatement = new Dictionary<string, decimal>(),
                IncomingTransfersStatement = new Dictionary<string, decimal>(),
                OutgoingTransfersSum = 0,
                IncomingTransfersSum = 0,
                MaxTransfer = 0,
                MinTransfer = 0,
                AverageTransfer = 0,
            };

            var account = new Account();
            var transferRepositoryMock = new Mock<ITransferRepository>();

            transferRepositoryMock
                .Setup(repo => repo.GetIncomingTransfers(account))
                .Returns(new List<Transfer>());
            transferRepositoryMock
                .Setup(repo => repo.GetOutgoingTransfers(account))
                .Returns(new List<Transfer>());

            var accountsService = new AccountsService(new TransfersService(transferRepositoryMock.Object),
                () => new BankAppDbContextMock());

            var statement = accountsService.CreateAccountStatement(account);

            statement.Should().BeEquivalentTo(accountStatementPattern);
        }

        [Test]
        public void CreateAccountStatement_OnlyIncomingTransfers_ReturnsCorrectIncomingSum()
        {
            var account1 = new Account(1, "acc1");
            var account2 = new Account(2, "acc2");
            var account3 = new Account(3, "acc3");

            var transfer1 = new Transfer()
            {
                Account = account2,
                Receiver = account1,
                Amount = 10.0m,
                Date = DateTime.Now,
                Name = "tr1",
                Type = TransferType.INCOMING
            };
            var transfer2 = new Transfer()
            {
                Account = account3,
                Receiver = account1,
                Amount = 20.0m,
                Date = DateTime.Now,
                Name = "tr2",
                Type = TransferType.INCOMING
            };

            var transferRepositoryMock = new Mock<ITransferRepository>();

            transferRepositoryMock
                .Setup(repo => repo.GetIncomingTransfers(account1))
                .Returns(new List<Transfer>() { transfer1, transfer2 });
            transferRepositoryMock
                .Setup(repo => repo.GetOutgoingTransfers(account1))
                .Returns(new List<Transfer>());

            var accountsService = new AccountsService(new TransfersService(transferRepositoryMock.Object),
                () => new BankAppDbContextMock());
            var statement = accountsService.CreateAccountStatement(account1);

            statement.IncomingTransfersSum.Should().Be(30.0m);

        }

        [Test]
        public void CreateAccountStatement_ExternalTransfersPerformed_ReturnsCorrectStatement()
        {
            var account1 = new Account(1, "acc1");
            var account2 = new Account(1, "acc2");
            var account3 = new Account(2, "acc3");

            var transfer1 = new Transfer()
            {
                Account = account1,
                Receiver = account2,
                Amount = 10.0m,
                Date = DateTime.Now,
                Name = "tr1",
                Type = TransferType.INTERNAL
            };
            var transfer2 = new Transfer()
            {
                Account = account1,
                Receiver = account3,
                Amount = 20.0m,
                Date = DateTime.Now,
                Name = "tr2",
                Type = TransferType.EXTERNAL
            };
            var transfer3 = new Transfer()
            {
                Account = account1,
                Receiver = account3,
                Amount = 30.0m,
                Date = DateTime.Now,
                Name = "tr3",
                Type = TransferType.EXTERNAL
            };

            var transferRepositoryMock = new Mock<ITransferRepository>();

            transferRepositoryMock
                .Setup(repo => repo.GetOutgoingTransfers(account1))
                .Returns(new List<Transfer>() { transfer1, transfer2, transfer3 });

            transferRepositoryMock
                .Setup(repo => repo.GetIncomingTransfers(account1))
                .Returns(new List<Transfer>());

            var accountsService = new AccountsService(new TransfersService(transferRepositoryMock.Object),
                () => new BankAppDbContextMock());


            var statement = accountsService.CreateAccountStatement(account1);

            statement.OutgoingTransfersStatement.Should()
                .Contain(new KeyValuePair<string, decimal>(account3.Number.ToString(), 50.0m));

        }
    }
}