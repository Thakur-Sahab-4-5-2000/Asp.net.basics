using StackExchange.Redis;
using System.Text.Json;
using Learning_Backend.Contracts;
using Learning_Backend.DTOS;

namespace Learning_Backend.Background_Service
{
    public class EmailQueueWorker : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EmailQueueWorker(IConnectionMultiplexer redis, IServiceScopeFactory serviceScopeFactory)
        {
            _redis = redis;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();

            while (!stoppingToken.IsCancellationRequested)
            {
                var emailTaskJson = await db.ListLeftPopAsync("taskQueue");

                if (!emailTaskJson.IsNullOrEmpty)
                {
                    var emailTask = JsonSerializer.Deserialize<EmailTaskDTO>(emailTaskJson);

                    if (emailTask != null)
                    {
                        var email = emailTask.email;
                        var subject = emailTask.subject;
                        var body = emailTask.body;

                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                            await emailSender.SendEmailAsync(email, subject, body);
                        }
                    }
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
