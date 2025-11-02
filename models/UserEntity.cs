namespace ft_kbe2c;

public class UserEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<TestEntity> Tests { get; set; } = [];
    public List<UserTestResultEntity> UserTestResults { get; set; } = [];
}