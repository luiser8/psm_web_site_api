using System.IdentityModel.Tokens.Jwt;
using psm_web_site_api_project.Dto;

namespace psm_web_site_api_project.Utils.JwtUtils;
public class JwtTokenMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userData = new UserTokenData
                {
                    IdUser = jwtToken.Claims.FirstOrDefault(c => c.Type == "iduser")?.Value,
                    FirstName = jwtToken.Claims.FirstOrDefault(c => c.Type == "firstname")?.Value,
                    LastName = jwtToken.Claims.FirstOrDefault(c => c.Type == "lastname")?.Value,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    Rol = jwtToken.Claims.FirstOrDefault(c => c.Type == "rol")?.Value,
                    Extensions = jwtToken.Claims.FirstOrDefault(c => c.Type == "extensions")?.Value,
                    Exp = long.Parse(jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value ?? "0")
                };

                context.Items["UserTokenData"] = userData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando JWT: {ex.Message}");
            }
        }

        await _next(context);
    }
}

public static class JwtTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtTokenProcessing(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtTokenMiddleware>();
    }

    public static UserTokenData? GetUserTokenData(this HttpContext context)
    {
        return context.Items["UserTokenData"] as UserTokenData;
    }
}