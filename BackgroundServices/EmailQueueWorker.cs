using StackExchange.Redis;
using System.Text.Json;
using Learning_Backend.Contracts;
using Microsoft.Extensions.DependencyInjection;

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
                var emailTaskJson = await db.ListLeftPopAsync("emailQueue");

                if (!emailTaskJson.IsNullOrEmpty)
                {
                    var emailTask = JsonSerializer.Deserialize<dynamic>(emailTaskJson);
                    var email = (string)emailTask.email;
                    var subject = (string)emailTask.subject;
                    var body = (string)emailTask.body;

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                        await emailSender.SendEmailAsync(email, subject, body);
                    }
                }

                await Task.Delay(5000, stoppingToken); // Delay before checking for new tasks
            }
        }
    }
}
