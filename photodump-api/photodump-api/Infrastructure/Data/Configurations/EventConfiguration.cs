using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db.Data.Configurations;

public class EventConfiguration: IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {   
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(300);
        
        builder.HasMany(e => e.GuestList)
            .WithOne(g => g.Event)
            .HasForeignKey(e => e.EventId);
    }
}