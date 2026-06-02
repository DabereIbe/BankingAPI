using BankingAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<DebitCard> DebitCards { get; set; }
    public DbSet<CardTransaction> CardTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.HasMany(e => e.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.DebitCards)
                .WithOne(d => d.Account)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.ReferenceNumber).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasOne(e => e.SenderAccount)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(e => e.SenderAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ReceiverAccount)
                .WithMany(a => a.ReceivedTransactions)
                .HasForeignKey(e => e.ReceiverAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DebitCard configuration
        modelBuilder.Entity<DebitCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CardNumber).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.CardNumber).IsUnique();
            entity.Property(e => e.CardholderName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ExpiryMonth).IsRequired().HasMaxLength(2);
            entity.Property(e => e.ExpiryYear).IsRequired().HasMaxLength(4);
            entity.Property(e => e.CVV).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CardType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DailyLimit).HasPrecision(18, 2);
            entity.Property(e => e.MonthlyLimit).HasPrecision(18, 2);
            entity.Property(e => e.DailySpent).HasPrecision(18, 2);
            entity.Property(e => e.MonthlySpent).HasPrecision(18, 2);
            entity.Property(e => e.BlockReason).HasMaxLength(500);
            entity.HasOne(e => e.Account)
                .WithMany(a => a.DebitCards)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.CardTransactions)
                .WithOne(ct => ct.DebitCard)
                .HasForeignKey(ct => ct.DebitCardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CardTransaction configuration
        modelBuilder.Entity<CardTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionReference).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.TransactionReference).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Merchant).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MerchantCategory).HasMaxLength(50);
            entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeclineReason).HasMaxLength(500);
            entity.HasOne(e => e.DebitCard)
                .WithMany(d => d.CardTransactions)
                .HasForeignKey(e => e.DebitCardId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}