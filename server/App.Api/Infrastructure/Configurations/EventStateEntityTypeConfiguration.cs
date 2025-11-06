using App.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Api.Infrastructure.Configurations;

public class EventStateEntityTypeConfiguration : IEntityTypeConfiguration<EventState>
{
    public void Configure(EntityTypeBuilder<EventState> builder)
    {
        builder
            .Property(et => et.StateName)
            .HasMaxLength(255)
            .IsRequired();
    }
}