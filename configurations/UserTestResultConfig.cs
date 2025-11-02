using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ft_kbe2c;

public class UserTestResultConfig : IEntityTypeConfiguration<UserTestResultEntity>
{
    public void Configure(EntityTypeBuilder<UserTestResultEntity> builder)
    {
        builder.HasKey(utr => utr.Id);
        builder.Property(utr => utr.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasOne(utr => utr.User).WithMany(u => u.UserTestResults).HasForeignKey(utr => utr.UserId);
        builder.HasOne(utr => utr.Test).WithMany(t => t.UserTestResults).HasForeignKey(utr => utr.TestId);
        builder.HasOne(utr => utr.TestResult).WithMany(tr => tr.UserTestResults).HasForeignKey(utr => utr.TestResultId);
    }
}