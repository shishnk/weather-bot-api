using DatabaseApp.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseApp.Persistence.EntityTypeConfiguration;

public class WeatherDescriptionConfiguration : IEntityTypeConfiguration<WeatherDescription>
{
    public void Configure(EntityTypeBuilder<WeatherDescription> builder)
    {
        builder.ToTable("WEATHER_DESCRIPTIONS");

        builder.Property(x => x.Id).HasColumnName("ID");
        builder.Property(x => x.Location).HasColumnName("LOCATION");

        builder.HasKey(subscription => subscription.Id);
        builder.Property(subscription => subscription.Id).ValueGeneratedOnAdd()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);
        builder.Property(subscription => subscription.Location).HasMaxLength(100);
    }
}