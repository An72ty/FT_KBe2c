using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ft_kbe2c;

public class TestResultConfig : IEntityTypeConfiguration<TestResultEntity>
{
    public void Configure(EntityTypeBuilder<TestResultEntity> builder)
    {
        builder.HasKey(tr => tr.Id);
        builder.Property(tr => tr.Text).HasMaxLength(50);
        builder.HasOne(tr => tr.Test).WithMany(t => t.TestResults).HasForeignKey(tr => tr.TestId);
        builder.HasMany(tr => tr.Answers).WithOne(a => a.TestResult);
        builder.HasMany(tr => tr.UserTestResults).WithOne(utr => utr.TestResult);
    }
}