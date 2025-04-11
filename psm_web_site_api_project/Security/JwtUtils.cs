using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;

namespace psm_web_site_api_project.Utils.JwtUtils;

public static class JwtUtils
{
    private static IConfiguration _configuration;

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private static string GetSetting(string key)
    {
        if (_configuration == null)
        {
            throw new InvalidOperationException("JwtUtils no ha sido inicializado. Llame a Initialize() primero.");
        }

        return _configuration[$"Security:Jwt:{key}"] ?? throw new ArgumentException($"No se encontró la configuración para 'Security:Jwt:{key}'");
    }

    public static string CreateToken(TokenPayload tokenDto)
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

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static string RefreshToken(TokenPayload tokenDto)
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

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}