﻿// <auto-generated />
using System;
using FileController.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileController.Migrations
{
    [DbContext(typeof(DbBankContext))]
    partial class DbBankContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("FileController.Models.AccountStatement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountIBAN")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<TimeOnly>("CreationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PagesText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValueEnd")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ValueStart")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountIBAN");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("AccountStatements");
                });

            modelBuilder.Entity("FileController.Models.BankAccount", b =>
                {
                    b.Property<string>("IBAN")
                        .HasColumnType("TEXT");

                    b.Property<string>("BIC")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("BLZ")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Bankname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.HasKey("IBAN");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("FileController.Models.StatementPage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountStatementId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountStatementId");

                    b.ToTable("StatementPage");
                });

            modelBuilder.Entity("FileController.Models.StatementTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountStatementId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SortOrder")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountStatementId");

                    b.ToTable("StatementTransaction");
                });

            modelBuilder.Entity("FileController.Models.AccountStatement", b =>
                {
                    b.HasOne("FileController.Models.BankAccount", "Account")
                        .WithMany("AccountStatements")
                        .HasForeignKey("AccountIBAN")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("FileController.Models.StatementPage", b =>
                {
                    b.HasOne("FileController.Models.AccountStatement", "AccountStatement")
                        .WithMany("Pages")
                        .HasForeignKey("AccountStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStatement");
                });

            modelBuilder.Entity("FileController.Models.StatementTransaction", b =>
                {
                    b.HasOne("FileController.Models.AccountStatement", "AccountStatement")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountStatementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountStatement");
                });

            modelBuilder.Entity("FileController.Models.AccountStatement", b =>
                {
                    b.Navigation("Pages");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("FileController.Models.BankAccount", b =>
                {
                    b.Navigation("AccountStatements");
                });
#pragma warning restore 612, 618
        }
    }
}
