using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ft_kbe2c;

[ApiController]
[Route("/")]
public class FtKbe2cController(IDbService dbService, JwtProvider jwtProvider) : ControllerBase
{
    private IDbService _dbService = dbService;
    private JwtProvider _jwtProvider = jwtProvider;


    [HttpGet]
    public async Task<IActionResult> GetTests()
    {
        List<TestEntity> tests = await _dbService.GetByPage<TestEntity>(1, 1000);
        string message = "";
        foreach (TestEntity test in tests)
        {
            message += $"{test.Name} ({test.Id}) - {test.Questions.Count} questions \nCreated by {test.User.Name} ({test.UserId}) ({test.CreatedAt})\n\n";
        }
        return Ok(message);
    }

    [HttpGet("test/{test_id}/")]
    public async Task<IActionResult> GetTestInfo(Guid test_id)
    {
        TestEntity test = await _dbService.GetOneByProperties<TestEntity>(new KeyValuePair<string, object>("Id", test_id));
        if (test is null) return NotFound();
        string message = $"{test.Name} ({test.Id}) - {test.Questions.Count} questions \nCreated by {test.User.Name} ({test.UserId}) ({test.CreatedAt})\n\nQuestions and possible answers:\n";
        foreach (QuestionEntity question in test.Questions)
        {
            message += $"{question.Text} ({question.Id})\n";
            foreach (AnswerEntity answer in question.Answers)
            {
                message += $" - {answer.Text} ({answer.Id})\n";
            }
        }
        message += "\nPossible Results:\n";
        foreach (TestResultEntity testResult in test.TestResults)
        {
            message += $"{testResult.Text} ({testResult.Id})\n";
        }

        return Ok(message);
    }

    [HttpPost("test/{test_id}/answer/")]
    [Authorize]
    public async Task<IActionResult> AnswerTest(Guid test_id, params KeyValuePair<Guid, Guid>[] answers)
    {
        Dictionary<Guid, Guid> answersDict = answers.ToDictionary();
        TestEntity test = await _dbService.GetOneByProperties<TestEntity>(new KeyValuePair<string, object>("Id", test_id));
        if (test is null) return NotFound();

        int i = 0;
        int[] testResultsPoints = new int[test.TestResults.Count];
        for (int c = 0; c < testResultsPoints.Length; c++)
        {
            testResultsPoints[c] = 0;
        }

        foreach (TestResultEntity testResult in test.TestResults)
        {
            foreach (KeyValuePair<Guid, Guid> QuestionAnswer in answersDict)
            {
                // AnswerEntity answer = await _dbService.GetOneByProperties<AnswerEntity>(new KeyValuePair<string, object>("Id", QuestionAnswer.Value));
                if (testResult.Answers.Any(x => x.Id == QuestionAnswer.Value))
                {
                    testResultsPoints[i] += 1;
                }
            }
        }
        int maxPoints = testResultsPoints.Max();
        int maxPointsIndex = testResultsPoints.ToList().IndexOf(maxPoints);

        return Ok(test.TestResults[maxPointsIndex].Text);
    }

    [HttpGet("user/{user_id}/")]
    public async Task<IActionResult> GetUserInfo(Guid user_id)
    {
        UserEntity user = await _dbService.GetOneByProperties<UserEntity>(new KeyValuePair<string, object>("Id", user_id));
        if (user is null) return NotFound();

        string message = "";
        message += $"{user.Name} ({user.Id})\nRegistered: {user.CreatedAt}\n\nTestResults:\n";
        foreach (UserTestResultEntity userTestResult in user.UserTestResults)
        {
            message += $"{userTestResult.Test.Name} ({userTestResult.Id}) - {userTestResult.TestResult.Test} ({userTestResult.Id}) ({userTestResult.CreatedAt})\n";
        }
        message += "\n\nCreated tests:\n";
        foreach (TestEntity test in user.Tests)
        {
            message += $"{test.Name} ({test.Id}) - {test.Questions.Count} questions ({test.CreatedAt})\n";
        }

        return Ok(message);
    }

    [HttpPost("create-test/")]
    [Authorize]
    public async Task<IActionResult> CreateTest(TestEntity test)
    {
        await _dbService.Add(test);
        return Ok();
    }

    [HttpPost("update-test/{test_id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTest(Guid test_id, TestEntity test)
    {
        await _dbService.Update(test_id, test);
        return Ok();
    }

    [HttpPost("register/")]
    public IActionResult Register(string name, string password)
    {
        _dbService.Add(new UserEntity { Name = name, PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password) });

        return Ok("You've successfuly registered. Now you need to login on 'login/' page");
    }

    [HttpPost("login/")]
    public async Task<IActionResult> Login(string name, string password)
    {
        UserEntity user = await _dbService.GetOneByProperties<UserEntity>(new KeyValuePair<string, object>("Name", name));
        if (user is null || !BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash)) return BadRequest("Name or password is incorrect");
        string token = _jwtProvider.GenerateToken(new KeyValuePair<string, string>("Id", user.Id.ToString()), new KeyValuePair<string, string>("Name", user.Name), new KeyValuePair<string, string>("PasswordHash", user.PasswordHash.ToString()));
        HttpContext.Response.Cookies.Append("zpftfkczsukf", token);
        return Ok("You've successfully authorized.");
    }
}