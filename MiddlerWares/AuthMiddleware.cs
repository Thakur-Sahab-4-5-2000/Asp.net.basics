using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

        if (allowAnonymous) {
            await _next(context);
            return;
        }
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("No token found in request.");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token is missing.");
            return;
        }

        try
        {
            var claimsPrincipal = ValidateToken(token);

            if (claimsPrincipal == null)
            {
                _logger.LogWarning("Invalid token.");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            context.User = claimsPrincipal;
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validating token: {ex.Message}");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An error occurred while validating the token.");
        }
    }

    private ClaimsPrincipal ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = "Shubham Thakur",
                ValidAudience = "Janleba",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("JWT_KEYS")))
            };

            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
