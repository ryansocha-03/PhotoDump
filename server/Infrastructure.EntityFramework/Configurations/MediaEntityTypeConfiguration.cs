using Core.Models;
using Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations;

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
            .HasDefaultValue(UploadStatus.Pending);

        builder
            .Property(m => m.DownloadCount)
            .HasDefaultValue(0)
            .IsRequired();
        
        builder
            .Property(m => m.IsPrivate)
            .HasDefaultValue(true)
            .IsRequired();
        
        builder
            .HasOne(m => m.MediaType)
            .WithMany(mt => mt.Media)
            .HasForeignKey(m => m.MediaTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(m => m.Event)
            .WithMany(e => e.Media)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}