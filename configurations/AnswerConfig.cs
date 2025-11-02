using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ft_kbe2c;

public class AnswerConfig : IEntityTypeConfiguration<AnswerEntity>
{
    public void Configure(EntityTypeBuilder<AnswerEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Text).HasMaxLength(1000);
        builder.HasOne(a => a.Question).WithMany(q => q.Answers).HasForeignKey(a => a.QuestionId);
        builder.HasOne(a => a.TestResult).WithMany(tr => tr.Answers).HasForeignKey(a => a.TestResultId);
    }
}