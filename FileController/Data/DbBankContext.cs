using Microsoft.EntityFrameworkCore;

namespace FileController.Data;
public class DbBankContext : DbContext
{
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<AccountStatement> AccountStatements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=BankLocalDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasIndex(e => e.Number).IsUnique();
        });

        modelBuilder.Entity<AccountStatement>(entity =>
        {
            entity.HasIndex(e => e.Number).IsUnique();
        });
    }
}
