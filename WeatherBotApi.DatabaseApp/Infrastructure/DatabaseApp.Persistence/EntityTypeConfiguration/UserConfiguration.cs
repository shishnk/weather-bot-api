using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseApp.Persistence.EntityTypeConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("USERS");

        builder.Property(user => user.Id).HasColumnName("ID");
        builder.Property(user => user.TelegramId).HasColumnName("TELEGRAM_ID");
        builder.Property(user => user.RegisteredAt).HasColumnName("TIMESTAMP");

        builder.OwnsOne(user => user.Metadata, metadata =>
        {
            metadata.Property(m => m.Username).HasColumnName("USERNAME").HasMaxLength(UserMetadata.MaxUsernameLength);
            metadata.Property(m => m.MobileNumber).HasColumnName("MOBILE_NUMBER");
        });

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedOnAdd()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);
    }
}