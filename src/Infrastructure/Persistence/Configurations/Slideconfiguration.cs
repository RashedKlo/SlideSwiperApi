using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SlideConfiguration : IEntityTypeConfiguration<Slide>
{
    public void Configure(EntityTypeBuilder<Slide> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Title1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Title2)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Title3Part1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Title3Part2)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Title3Part3)
            .HasMaxLength(200);

        builder.Property(s => s.Title4)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Order)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Index for ordering slides efficiently
        builder.HasIndex(s => s.Order);
    }
}