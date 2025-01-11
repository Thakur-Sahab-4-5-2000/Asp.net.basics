using StackExchange.Redis;
using System.Text.Json;
using Learning_Backend.Contracts;

public class EmailQueueWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IEmailSender _emailSender;

    public EmailQueueWorker(IConnectionMultiplexer redis, IEmailSender emailSender)
    {
        _redis = redis;
        _emailSender = emailSender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            var emailTaskJson = await db.ListLeftPopAsync("emailQueue");

            if (!emailTaskJson.IsNullOrEmpty)
            {
                var emailTask = JsonSerializer.Deserialize<dynamic>(emailTaskJson);
                var email = (string)emailTask.email;
                var subject = (string)emailTask.subject;
                var body = (string)emailTask.body;
                await _emailSender.SendEmailAsync(email, subject, body);
            }

            await Task.Delay(5000, stoppingToken); 
        }
    }
}
