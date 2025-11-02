using Microsoft.EntityFrameworkCore;

namespace ft_kbe2c;

public class FtKbe2cDbContext(DbContextOptions<FtKbe2cDbContext> options) : DbContext(options)
{
    public DbSet<AnswerEntity> Answers { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    public DbSet<TestEntity> Tests { get; set; }
    public DbSet<TestResultEntity> TestResults { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserTestResultEntity> UserTestResults { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ft_kbe2c;Username=postgres;Password=root")
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AnswerConfig());
        modelBuilder.ApplyConfiguration(new QuestionConfig());
        modelBuilder.ApplyConfiguration(new TestConfig());
        modelBuilder.ApplyConfiguration(new TestResultConfig());
        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new UserTestResultConfig());
    }
}