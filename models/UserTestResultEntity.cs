namespace ft_kbe2c;

public class UserTestResultEntity : IEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid TestId { get; set; }
    public Guid TestResultId { get; set; }
    public UserEntity User { get; set; }
    public TestEntity Test { get; set; }
    public TestResultEntity TestResult { get; set; }
}