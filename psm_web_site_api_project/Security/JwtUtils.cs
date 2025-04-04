using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Utils.JwtUtils;
public static class JwtUtils
{
    private static IConfiguration? _configuration;

    public static IConfiguration Configuration
    {
        get
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            _configuration = builder.Build();
            return _configuration;
        }
    }

    public static string GetSetting(string key)
    {
        return Configuration.GetSection($"Security:Jwt:{key}").Value ?? string.Empty;
    }

    public static string CreateToken(TokenDto tokenDto)
    {
        ArgumentNullException.ThrowIfNull(tokenDto);

        var claims = new List<Claim>
        {
            new("iduser", tokenDto.IdUsuario?.ToString() ?? string.Empty),
            new("firstname", tokenDto.Nombres ?? string.Empty),
            new("lastname", tokenDto.Apellidos ?? string.Empty),
            new("email", tokenDto.Correo ?? string.Empty),
            new("rol", (tokenDto.Rol?.IdRol + "-" + tokenDto.Rol?.Nombre) ?? string.Empty)
        };

        foreach (var extension in tokenDto.Extension ?? Array.Empty<Extension>())
        {
            claims.Add(new Claim("extensions", extension.IdExtension + "-" + extension.Nombre));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSetting("Token")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToInt16(GetSetting("ExpirationToken"))),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return accessToken;
    }

    public static string RefreshToken(TokenDto tokenDto)
    {
        ArgumentNullException.ThrowIfNull(tokenDto);
        var claims = new List<Claim>
        {
            new("iduser", tokenDto.IdUsuario?.ToString() ?? string.Empty),
            new("firstname", tokenDto.Nombres ?? string.Empty),
            new("lastname", tokenDto.Apellidos ?? string.Empty),
            new("email", tokenDto.Correo ?? string.Empty),
            new("rol", (tokenDto.Rol?.IdRol + "-" + tokenDto.Rol?.Nombre) ?? string.Empty)
        };

        foreach (var extension in tokenDto.Extension ?? Array.Empty<Extension>())
        {
            claims.Add(new Claim("extensions", extension.IdExtension + "-" + extension.Nombre));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSetting("Token")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(Convert.ToInt16(GetSetting("ExpirationRefreshToken"))),
            signingCredentials: creds);

        var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return refreshToken;
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}
