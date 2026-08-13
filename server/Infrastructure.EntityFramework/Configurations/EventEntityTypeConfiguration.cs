using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

/// <summary>
/// EF core entity configurations for Event entities.
/// </summary>
public class EventEntityTypeConfiguration :  IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .Property(e => e.PublicId)
            .IsRequired()
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();
        
        builder
            .HasIndex(e => e.PublicId)
            .IsUnique();

        builder
            .Property(e => e.EventName)
            .IsRequired();
        
        builder
            .Property(e => e.EventNameShort)
            .HasMaxLength(25);

        builder
            .Property(e => e.ColorPrimary)
            .HasMaxLength(7);
        
        builder
            .Property(e => e.ColorSecondary)
            .HasMaxLength(7);
        
        builder
            .Property(e => e.StartDate)
            .IsRequired();

        builder
            .Property(e => e.EndDate)
            .IsRequired();
        
        builder
            .Property(e => e.EventPasswordHash)
            .IsRequired();

        builder
            .Property(e => e.EventState)
            .IsRequired()
            .HasDefaultValue(EventStateEnum.Draft);
        
        builder
            .HasOne(e => e.EventType)
            .WithMany(et => et.Events)
            .HasForeignKey(e => e.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}