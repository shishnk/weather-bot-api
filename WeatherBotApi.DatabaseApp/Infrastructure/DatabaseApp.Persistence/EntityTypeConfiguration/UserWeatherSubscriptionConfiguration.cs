using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseApp.Persistence.EntityTypeConfiguration;

public class UserWeatherSubscriptionConfiguration : IEntityTypeConfiguration<UserWeatherSubscription>
{
    public void Configure(EntityTypeBuilder<UserWeatherSubscription> builder)
    {
        builder.ToTable("WEATHER_SUBSCRIPTIONS");

        builder.Property(uws => uws.Id).HasColumnName("ID");

        builder.HasKey(uws => uws.Id);
        builder.Property(uws => uws.Id).ValueGeneratedOnAdd()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);

        builder.Property(uws => uws.UserId).HasColumnName("USER_ID");
        builder.Property(uws => uws.WeatherDescriptionId).HasColumnName("WEATHER_DESCRIPTION_ID");

        builder
            .HasOne(uws => uws.User)
            .WithMany()
            .HasForeignKey(uws => uws.UserId)
            .OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_USER_ID");
        builder
            .HasOne(uws => uws.WeatherDescription)
            .WithMany()
            .HasForeignKey(uws => uws.WeatherDescriptionId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_WEATHER_DESCRIPTION_ID");
    }
}