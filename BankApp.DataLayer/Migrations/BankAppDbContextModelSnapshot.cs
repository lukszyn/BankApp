﻿// <auto-generated />
using System;
using BankApp.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankApp.DataLayer.Migrations
{
    [DbContext(typeof(BankAppDbContext))]
    partial class BankAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BankApp.DataLayer.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<decimal?>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("Number")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("ExternalReceiverNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ReceiverId");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.Account", b =>
                {
                    b.HasOne("BankApp.DataLayer.Models.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.Transfer", b =>
                {
                    b.HasOne("BankApp.DataLayer.Models.Account", "Account")
                        .WithMany("OutgoingTransfers")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BankApp.DataLayer.Models.Account", "Receiver")
                        .WithMany("IncomingTransfers")
                        .HasForeignKey("ReceiverId");

                    b.Navigation("Account");

                    b.Navigation("Receiver");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.Account", b =>
                {
                    b.Navigation("IncomingTransfers");

                    b.Navigation("OutgoingTransfers");
                });

            modelBuilder.Entity("BankApp.DataLayer.Models.User", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
