using Learning_Backend.Contracts;
using Learning_Backend.Databases;
using Learning_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using Microsoft.Extensions.FileProviders;
using Learning_Backend.Background_Service;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<LearningDatabase>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ),
    ServiceLifetime.Scoped
);
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddHostedService<EmailQueueWorker>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_KEYS"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IRepoWrapper, RepoWrapper>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddHostedService<EmailQueueWorker>();


builder.Services.AddControllers();

builder.Services.AddLogging();

var app = builder.Build();

// Remove the Swagger-related middleware
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

app.UseDefaultFiles();   // Serve index.html as the default file
app.UseStaticFiles();    // Serve static files from wwwroot folder

app.UseMiddleware<AuthMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
