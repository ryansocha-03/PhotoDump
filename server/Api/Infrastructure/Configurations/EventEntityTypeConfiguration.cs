using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Configurations;

public class EventEntityTypeConfiguration :  IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder
            .Property(e => e.EventName)
            .IsRequired();

        builder
            .Property(e => e.EventDate)
            .IsRequired();
        
        builder
            .Property(e => e.EventPasswordHash)
            .IsRequired();

        builder
            .HasOne(e => e.EventState)
            .WithMany(es => es.Events)
            .HasForeignKey(e => e.EventStateId);
        
        builder
            .HasOne(e => e.EventType)
            .WithMany(et => et.Events)
            .HasForeignKey(e => e.EventTypeId);

        builder
            .HasMany(e => e.Guests)
            .WithOne(g => g.Event)
            .HasForeignKey(g => g.EventId);
        
        builder
            .HasMany(e => e.Admins)
            .WithOne(a => a.Event)
            .HasForeignKey(a => a.EventId);
    }
}