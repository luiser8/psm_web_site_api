using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using psm_web_site_api_project.Utils.Mappers;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Repository.Usuarios;
using psm_web_site_api_project.Services.Usuarios;
using psm_web_site_api_project.Repository.Roles;
using psm_web_site_api_project.Repository.Extensiones;
using psm_web_site_api_project.Repository.Auditorias;
using psm_web_site_api_project.Services.Roles;
using psm_web_site_api_project.Services.Extensiones;
using psm_web_site_api_project.Services.Redis;
using psm_web_site_api_project.Security.Headers;
using AspNetCoreRateLimit;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Repository.Autenticacion;
using psm_web_site_api_project.Repository.Headers;
using psm_web_site_api_project.Repository.ImageUpAndDown;
using psm_web_site_api_project.Services.Autenticacion;
using psm_web_site_api_project.Utils.JsonArrayModelBinder;
using psm_web_site_api_project.Utils.JwtUtils;
using System.Text.Json;
using psm_web_site_api_project.Services.Headers;
using psm_web_site_api_project.Repository.CarouselRepository;
using psm_web_site_api_project.Services.Carousel;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
}

builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddMvc();

if (builder.Environment.IsProduction())
{
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
}

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers(options =>
            {
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status404NotFound));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
                options.Filters.Add(
                    new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            }).AddNewtonsoftJson()
            .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<ConfigDB>(builder.Configuration.GetSection("Clients:MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<ConfigDB>>().Value;
    return new MongoClient(config.ConnectionString);
});
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration["Security:Jwt:Token"] ?? string.Empty)),
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            try
            {
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAutenticacionService>();
                var jwtToken = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(jwtToken))
                {
                    context.Fail("Token inválido");
                    return;
                }

                var userId = context.Principal?.Claims.FirstOrDefault(c => c.Type == "iduser")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    context.Fail("Usuario no identificado");
                    return;
                }

                var isTokenValid = await authService.ValidarService(userId, jwtToken);

                if (!isTokenValid)
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    context.Fail("Token revocado");
                }
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error validando token");
                context.Fail("Error validando token");
            }
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (context.AuthenticateFailure != null)
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var errorMessage = context.AuthenticateFailure switch
                {
                    SecurityTokenExpiredException => "Token expirado",
                    _ => "Token inválido o no autorizado"
                };
                return context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    StatusCode = 401,
                    Message = errorMessage,
                    Error = context.AuthenticateFailure.Message
                }));
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddCors(options =>
    {
        options.AddPolicy(myAllowSpecificOrigins,
            builder =>
                {
                    builder
                        .WithOrigins("*")
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                        .WithExposedHeaders("X-Custom-Header");
                });
    });
builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRepository>();
builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();

builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IRolesService, RolesService>();

builder.Services.AddScoped<IExtensionesRepository, ExtensionesRepository>();
builder.Services.AddScoped<IExtensionesService, ExtensionesService>();

builder.Services.AddScoped<IHeaderRepository, HeaderRepository>();
builder.Services.AddScoped<IHeaderService, HeaderService>();

builder.Services.AddScoped<ICarouselRepository, CarouselRepository>();
builder.Services.AddScoped<ICarouselService, CarouselService>();

builder.Services.AddScoped<IAuditoriasRepository, AuditoriasRepository>();

builder.Services.AddScoped<IImageUpAndDownService, ImageUpAndDownService>();

builder.Services.AddScoped<IJwtUtils, JwtUtils>();

builder.Services.AddSwaggerGen(c =>
{
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
    c.MapType<HeaderCollection>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>()
    });
});

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new JsonArrayModelBinderProvider());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(myAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseJwtTokenProcessing();
app.UseAuthorization();
app.UseMiddleware<SecurityHeaders>();
if (app.Environment.IsProduction())
    app.UseIpRateLimiting();
app.MapControllers();

await app.RunAsync();