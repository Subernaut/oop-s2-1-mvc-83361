using Library.Domain;

public class ExamService
{
    private readonly List<Exam> _exams = new();
    private int _nextId = 1;

    public Exam CreateExam(string title, int courseId, DateTime date, int maxScore, bool resultsReleased)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.");
        if (maxScore <= 0)
            throw new ArgumentException("MaxScore must be positive.");

        var exam = new Exam
        {
            Id = _nextId++,
            Title = title,
            CourseId = courseId,
            Date = date,
            MaxScore = maxScore,
            ResultsReleased = resultsReleased
        };

        _exams.Add(exam);
        return exam;
    }

    public Exam? GetExamById(int id) => _exams.FirstOrDefault(e => e.Id == id);

    public List<Exam> GetAllExams() => _exams.ToList();

    public void SetResultsReleased(int id, bool released)
    {
        var exam = GetExamById(id);
        if (exam != null)
            exam.ResultsReleased = released;
    }
}