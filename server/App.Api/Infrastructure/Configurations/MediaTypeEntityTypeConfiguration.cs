using App.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Api.Infrastructure.Configurations;

public class MediaTypeEntityTypeConfiguration : IEntityTypeConfiguration<MediaType>
{
    public void Configure(EntityTypeBuilder<MediaType> builder)
    {
        builder
            .Property(mt => mt.FileExtension)
            .HasMaxLength(24)
            .IsRequired();
    }
}