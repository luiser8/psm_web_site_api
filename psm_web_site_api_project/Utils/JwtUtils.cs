using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using psm_web_site_api_project.Dto;

namespace psm_web_site_api_project.Utils.JwtUtils;
public static class JwtUtils
{
    private static IConfiguration? configuration;

    public static IConfiguration Configuration
    {
        get
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            configuration = builder.Build();
            return configuration;
        }
    }

    public static string GetSetting()
    {
        return Configuration.GetSection("AppSettings:Token").Value;
    }

    public static string CreateToken(TokenDto tokenDto)
    {
        var claims = new List<Claim>
        {
            new Claim("iduser", tokenDto.IdUsuario.ToString()),
            new Claim("firstname", tokenDto.Nombres),
            new Claim("lastname", tokenDto.Apellidos),
            new Claim(ClaimTypes.Email, tokenDto.Correo)
        };

        foreach (var rol in tokenDto.Roles)
        {
            claims.Add(new Claim("Rol", rol?.IdRol + "-" + rol.Nombre));
        }

        foreach (var extension in tokenDto.Extension)
        {
            claims.Add(new Claim("Extension", extension.IdExtension + "-" + extension.Nombre));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSetting()));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return accessToken;
    }

    public static string RefreshToken(TokenDto tokenDto)
    {
        var claims = new List<Claim>
        {
            new Claim("iduser", tokenDto.IdUsuario.ToString()),
            new Claim("firstname", tokenDto.Nombres),
            new Claim("lastname", tokenDto.Apellidos),
            new Claim(ClaimTypes.Email, tokenDto.Correo)
        };

        foreach (var rol in tokenDto.Roles)
        {
            claims.Add(new Claim("Rol", rol?.IdRol + "-" + rol.Nombre));
        }

        foreach (var extension in tokenDto.Extension)
        {
            claims.Add(new Claim("Extension", extension.IdExtension + "-" + extension.Nombre));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSetting()));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds);

        var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return refreshToken;
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}
