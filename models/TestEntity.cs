using Microsoft.AspNetCore.SignalR;

namespace ft_kbe2c;


public class TestEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? UserId { get; set; }
    public virtual UserEntity? User { get; set; }
    public virtual List<QuestionEntity> Questions { get; set; }
    public virtual List<TestResultEntity>? TestResults { get; set; }
    public virtual List<UserTestResultEntity> UserTestResults { get; set; } = [];
}