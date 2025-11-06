namespace ft_kbe2c;

public class QuestionEntity : IEntity
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid TestId { get; set; }
    public virtual TestEntity Test { get; set; }
    public virtual List<AnswerEntity> Answers { get; set; }
}