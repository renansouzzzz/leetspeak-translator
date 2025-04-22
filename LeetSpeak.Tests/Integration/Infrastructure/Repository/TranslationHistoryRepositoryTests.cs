using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

public class TranslationHistoryRepositoryTests
{
    private readonly TranslationHistoryRepository _repository;
    private readonly string _userId = "21781553-e3f9-46e4-9667-2915dcda0551";

    public TranslationHistoryRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\LeetSpeak.Web")) 
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString))
            .Options);

        _repository = new TranslationHistoryRepository(context, configuration);

    }

    private HistoryFilter filter => new HistoryFilter
    {
        SearchTerm = "l33t",
        StartDate = DateTime.UtcNow.AddDays(-10),
        EndDate = DateTime.UtcNow.AddDays(1)
    };

    [Fact(DisplayName = "Must apply filter of database procedure")]
    public async Task GetUserHistoryAsync_ShouldApplyFiltersCorrectly()
    {
        var result = await _repository.GetUserHistoryViaStoredProcAsync(filter, _userId);

        Assert.NotNull(result);
        Assert.True(result.Any(), "No results returned. Please check the data in the database.");

        foreach (var item in result)
        {
            Assert.True(
                item.OriginalText.Contains(filter.SearchTerm!, StringComparison.OrdinalIgnoreCase) ||
                item.TranslatedText.Contains(filter.SearchTerm!, StringComparison.OrdinalIgnoreCase));
            Assert.Equal(_userId, item.UserId);
            Assert.True(item.TranslationDate >= filter.StartDate && item.TranslationDate <= filter.EndDate);
        }
    }

    [Fact(DisplayName = "Should handle exceptions gracefully")]
    public async Task GetUserHistoryAsync_ShouldHandleException()
    {
        var mockRepo = new Mock<ITranslationHistoryRepository>();
        var mockLogger = new Mock<ILogger<TranslationHistoryService>>();
        var service = new TranslationHistoryService(mockRepo.Object, mockLogger.Object);

        mockRepo.Setup(x => x.GetUserHistoryViaStoredProcAsync(It.IsAny<HistoryFilter>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        var loggedMessages = new List<string>();
        _ = mockLogger.Setup(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((_, _) => true)))
            .Callback(new InvocationAction(invocation =>
            {
                var logLevel = (LogLevel)invocation.Arguments[0];
                var exception = (Exception)invocation.Arguments[3];
                loggedMessages.Add($"{logLevel}: {exception.Message}");
            }));

        var result = await service.GetUserHistoryAsync(filter, _userId);

        Assert.Empty(result);
        Assert.Single(loggedMessages);
        Assert.Contains("Database error", loggedMessages[0]);
    }
}