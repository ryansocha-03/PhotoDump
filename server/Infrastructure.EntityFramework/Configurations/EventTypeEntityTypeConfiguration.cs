using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configurations for Event Type entities.
/// </summary>
public class EventTypeEntityTypeConfiguration : IEntityTypeConfiguration<EventType>
{
    public void Configure(EntityTypeBuilder<EventType> builder)
    {
        builder
            .Property(et => et.TypeName)
            .IsRequired()
            .HasMaxLength(255);
    }
}