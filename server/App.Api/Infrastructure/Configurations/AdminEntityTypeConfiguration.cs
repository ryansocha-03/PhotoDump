using App.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Api.Infrastructure.Configurations;

public class AdminEntityTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder
            .Property(a => a.FirstName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder
            .Property(a => a.LastName)
            .HasMaxLength(255)
            .IsRequired();
    }
}