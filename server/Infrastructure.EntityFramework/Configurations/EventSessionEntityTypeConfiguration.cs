using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configurations for Event Session entities.
/// </summary>
public class EventSessionEntityTypeConfiguration: IEntityTypeConfiguration<EventSession>
{
    public void Configure(EntityTypeBuilder<EventSession> builder)
    {
        builder
            .HasKey(e => e.Id);
        
        builder
            .Property(es => es.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder
            .HasOne(es => es.Event)
            .WithMany(e => e.EventSession)
            .HasForeignKey(e => e.EventPublicId)
            .HasPrincipalKey(e => e.PublicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(es => es.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd();

        builder
            .Property(es => es.ExpiresAt)
            .IsRequired();

        builder
            .Property(es => es.UserAgent)
            .HasMaxLength(500);
    }
}