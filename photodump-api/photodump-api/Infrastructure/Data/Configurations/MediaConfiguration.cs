using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photodump_api.Domain;

namespace photodump_api.Infrastructure.Data.Configurations;

public class MediaConfiguration: IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Path).IsRequired();

        builder.HasOne(m => m.Guest)
            .WithMany(g => g.MediaUploads)
            .HasForeignKey(m => m.GuestId);
        
        builder.HasOne(m => m.Event)
            .WithMany(e => e.MediaList)
            .HasForeignKey(m => m.EventId);
        
        builder.Property(m => m.IsPrivate)
            .HasDefaultValue(true);

        builder.Property(m => m.UploadDate)
            .HasDefaultValue(DateTime.UtcNow);
    }
}