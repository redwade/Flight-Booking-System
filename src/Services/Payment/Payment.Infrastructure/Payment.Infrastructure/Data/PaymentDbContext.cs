using Microsoft.EntityFrameworkCore;
using Payment.Core.Entities;

namespace Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentEntity> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
            entity.HasIndex(e => e.BookingId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TransactionId);
        });
    }
}
