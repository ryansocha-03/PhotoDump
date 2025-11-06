using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class GuestEntityTypeConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder
            .Property(g => g.FirstName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property(g => g.LastName)
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .HasMany(g => g.UploadedMedia)
            .WithOne(u => u.Guest)
            .HasForeignKey(u => u.GuestId);
    }
}