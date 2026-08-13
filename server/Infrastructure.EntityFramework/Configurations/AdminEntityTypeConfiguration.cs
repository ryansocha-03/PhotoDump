using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configuration for Admin entities.
/// </summary>
public class AdminEntityTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder
            .Property(a => a.FirstName)
            .HasMaxLength(100);

        builder
            .Property(a => a.LastName)
            .HasMaxLength(255);
        
        builder
            .HasOne(a => a.Event)
            .WithMany(e => e.Admins)
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}