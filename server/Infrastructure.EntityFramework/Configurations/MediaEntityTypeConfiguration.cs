using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

/// <summary>
/// EF Core entity configurations for Media entities.
/// </summary>
public class MediaEntityTypeConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder
            .Property(m => m.FileName)
            .IsRequired();
        
        builder
            .Property(m => m.PublicFileName)
            .HasMaxLength(64)
            .IsRequired();
        
        builder
            .Property(m => m.OriginalSize)
            .IsRequired();

        builder
            .Property(m => m.Status)
            .IsRequired()
            .HasDefaultValue(ContentStatusEnum.Pending);
        
        builder
            .Property(m => m.UploadAttempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(m => m.DownloadCount)
            .HasDefaultValue(0)
            .IsRequired();
        
        builder
            .Property(m => m.IsPrivate)
            .HasDefaultValue(true)
            .IsRequired();

        builder
            .Property(m => m.ContentType)
            .IsRequired()
            .HasDefaultValue(ContentTypeEnum.None);
        
        builder
            .HasOne(m => m.Event)
            .WithMany(e => e.Media)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}