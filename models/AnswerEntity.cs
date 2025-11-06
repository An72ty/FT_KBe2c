namespace ft_kbe2c;

public class AnswerEntity : IEntity
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public int PointsCost { get; set; } = 1;
    public Guid QuestionId { get; set; }
    public Guid? TestResultId { get; set; }
    public virtual QuestionEntity Question { get; set; }
    public virtual TestResultEntity? TestResult { get; set; }
}