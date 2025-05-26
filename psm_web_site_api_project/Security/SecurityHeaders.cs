namespace psm_web_site_api_project.Security.Headers;

public class SecurityHeaders(RequestDelegate next)
{
    public Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Referrer-Policy", "no-referrer");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        context.Response.Headers.Append("Cache-Control", "no-cache, no-store");
        context.Response.Headers.Append("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");
        context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
        context.Response.Headers.Append("Cross-Origin-Resource-Policy", "same-origin");
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");
        return next.Invoke(context);
    }
}