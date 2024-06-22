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

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
builder.Services.Configure<ConfigDB>(builder.Configuration.GetSection("MongoDataBase"));
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };
    });
builder.Services.AddCors(options =>
    {
        options.AddPolicy(MyAllowSpecificOrigins,
            builder =>
                {
                    builder
                        .WithOrigins("*")
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH");
                });
    });
builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IRolesService, RolesService>();

builder.Services.AddScoped<IExtensionesRepository, ExtensionesRepository>();
builder.Services.AddScoped<IExtensionesService, ExtensionesService>();

builder.Services.AddScoped<IAuditoriasRepository, AuditoriasRepository>();

// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration["Clients:Redis:host"];
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SecurityHeaders>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseCors(MyAllowSpecificOrigins);
app.Run();