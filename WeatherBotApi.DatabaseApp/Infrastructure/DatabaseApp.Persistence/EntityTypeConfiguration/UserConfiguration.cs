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
        builder.Property(user => user.Username).HasColumnName("USERNAME");
        builder.Property(user => user.MobileNumber).HasColumnName("MOBILE_NUMBER");
        builder.Property(user => user.RegisteredAt).HasColumnName("TIMESTAMP");

        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedOnAdd()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);
    }
}