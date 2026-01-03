using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

public class EventSessionEntityTypeConfiguration: IEntityTypeConfiguration<EventSession>
{
    public void Configure(EntityTypeBuilder<EventSession> builder)
    {
        builder
            .Property(es => es.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();

        builder
            .HasOne<Event>()
            .WithMany()
            .HasForeignKey(e => e.EventPublicId)
            .HasPrincipalKey(e => e.PublicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Guest>()
            .WithMany()
            .HasForeignKey(e => e.GuestId)
            .HasPrincipalKey(g => g.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(es => es.UserAgent)
            .HasMaxLength(500)
            .HasColumnType("text");
    }
}