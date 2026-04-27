using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Infrastructure.Persistence.Configurations;

public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currency");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.CharCode)
            .HasColumnName("char_code")
            .HasMaxLength(8)
            .IsRequired();
        builder.HasIndex(x => x.CharCode).IsUnique();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasColumnName("rate")
            .HasColumnType("numeric(18,6)")
            .IsRequired();

        builder.Property(x => x.Nominal)
            .HasColumnName("nominal")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
