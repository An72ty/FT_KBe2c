using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ft_kbe2c;

public class TestConfig : IEntityTypeConfiguration<TestEntity>
{
    public void Configure(EntityTypeBuilder<TestEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(50);
        builder.Property(t => t.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasOne(t => t.User).WithMany(u => u.Tests).HasForeignKey(t => t.UserId);
        builder.HasMany(t => t.Questions).WithOne(q => q.Test);
        builder.HasMany(t => t.TestResults).WithOne(tr => tr.Test);
        builder.HasMany(t => t.UserTestResults).WithOne(utr => utr.Test);
    }
}