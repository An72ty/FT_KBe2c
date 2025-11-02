using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ft_kbe2c;

public class QuestionConfig : IEntityTypeConfiguration<QuestionEntity>
{
    public void Configure(EntityTypeBuilder<QuestionEntity> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).HasMaxLength(100);
        builder.HasOne(q => q.Test).WithMany(t => t.Questions).HasForeignKey(q => q.TestId);
        builder.HasMany(q => q.Answers).WithOne(a => a.Question);
    }
}