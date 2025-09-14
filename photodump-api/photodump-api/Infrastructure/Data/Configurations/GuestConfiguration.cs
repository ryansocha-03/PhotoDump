using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db.Data.Configurations;

public class GuestConfiguration: IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(g => g.LastName).IsRequired().HasMaxLength(50);
    }
}