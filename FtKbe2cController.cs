using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        message += "\nPassed the test:\n";
        foreach (UserTestResultEntity userTestResult in test.UserTestResults)
        {
            message += $"{userTestResult.User.Name} ({userTestResult.User.Id}) - '{userTestResult.TestResult.Text}' ({userTestResult.TestResult.Id}) ({userTestResult.CreatedAt})\n";
        }

        return Ok(message);
    }

    [HttpPost("test/{test_id}/answer/")]
    [Authorize]
    public async Task<IActionResult> AnswerTest(Guid test_id, params KeyValuePair<Guid, Guid>[] answers)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jsonToken = handler.ReadJwtToken(HttpContext.Request.Cookies["zpftfkczsukf"]);
        var idToken = jsonToken.Claims.FirstOrDefault(c => c.Type == "Id");
        UserEntity user = await _dbService.GetOneByProperties<UserEntity>(new KeyValuePair<string, object>("Id", idToken.Value));
        if (user is null) return Unauthorized();

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
                    testResultsPoints[i] += testResult.Answers.FirstOrDefault(x => x.Id == QuestionAnswer.Value).PointsCost;
                }
            }
            i++;
        }
        int maxPoints = testResultsPoints.Max();
        int maxPointsIndex = testResultsPoints.ToList().IndexOf(maxPoints);
        // TestResultEntity result = await _dbService.GetOneByProperties<TestResultEntity>(new KeyValuePair<string, object>("Id", test.TestResults[maxPointsIndex].Id));
        TestResultEntity result = test.TestResults[maxPointsIndex];

        UserTestResultEntity userTestResult = new UserTestResultEntity(){UserId=user.Id, TestId=test.Id, TestResultId=result.Id};

        List<UserTestResultEntity> uutr = user.UserTestResults;
        List<UserTestResultEntity> tutr = test.UserTestResults;
        List<UserTestResultEntity> rutr = result.UserTestResults;
        uutr.Append(userTestResult);
        tutr.Append(userTestResult);
        rutr.Append(userTestResult);
        await _dbService.Add(userTestResult);
        await _dbService.UpdateByProperties<UserEntity>(user.Id, new KeyValuePair<string, object>("UserTestResults", uutr));
        await _dbService.UpdateByProperties<TestEntity>(test.Id, new KeyValuePair<string, object>("UserTestResults", tutr));
        await _dbService.UpdateByProperties<TestResultEntity>(result.Id, new KeyValuePair<string, object>("UserTestResults", rutr));
 
        return Ok(result.Text);
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
            message += $"{userTestResult.Test.Name} ({userTestResult.Test.Id}) - '{userTestResult.TestResult.Text}' ({userTestResult.TestResult.Id}) ({userTestResult.CreatedAt})\n";
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
    public async Task<IActionResult> Register(string name, string password)
    {
        await _dbService.Add(new UserEntity { Name = name, PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password) });

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