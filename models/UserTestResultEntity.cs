namespace ft_kbe2c;

public class UserTestResultEntity : IEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TestId { get; set; }
    public Guid? TestResultId { get; set; }
    public virtual UserEntity? User { get; set; }
    public virtual TestEntity? Test { get; set; }
    public virtual TestResultEntity? TestResult { get; set; }
}