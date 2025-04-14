using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Utils.JwtUtils;
using psm_web_site_api_project.Utils.Md5utils;
using psm_web_site_api_project.Entities;
using psm_web_site_api_project.Payloads;

namespace psm_web_site_api_project.Repository.Autenticacion;
public class AutenticacionRepository : IAutenticacionRepository
{
    private readonly IMongoCollection<Usuario> _usuariosCollection;

    public AutenticacionRepository(IOptions<ConfigDB> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _usuariosCollection = mongoDatabase.GetCollection<Usuario>("usuarios");
    }

    public async Task<Usuario> SessionRepository(LoginPayload loginPayloadDto)
    {
        try
        {
            var response = await _usuariosCollection.Find(driver => driver.Correo == loginPayloadDto.Correo && driver.Contrasena == Md5utilsClass.GetMd5(loginPayloadDto.Contrasena ?? string.Empty)).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Usuario no encontrado, por favor verifica los datos correctamente");

            if (response.Activo == false)
                throw new Exception("Usuario deshabilitado");

            var newAccessToken = JwtUtils.CreateToken(new TokenPayload { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });
            var newRefreshToken = JwtUtils.RefreshToken(new TokenPayload { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });

            response.TokenAcceso = newAccessToken;
            response.TokenRefresco = newRefreshToken;
            response.TokenCreado = DateTime.Now;
            response.TokenExpiracion = DateTime.Now.AddDays(7);

            return response;

        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Usuario> RefrescoRepository(string refreshToken)
    {
        try
        {
            var isTokenValidTime = await ValidateRefreshTokenBeforeDb(refreshToken);
            if(isTokenValidTime)
                throw new Exception("Token refresh expirado");

            var response = await _usuariosCollection.Find(driver => driver.TokenRefresco == refreshToken).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Token refresh not found");

            if (response.Activo == false)
                throw new Exception("User status disabled");

            var newAccessToken = JwtUtils.CreateToken(new TokenPayload { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });
            var newRefreshToken = JwtUtils.RefreshToken(new TokenPayload { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });

            response.TokenAcceso = newAccessToken;
            response.TokenRefresco = newRefreshToken;
            response.TokenCreado = DateTime.Now;
            response.TokenExpiracion = DateTime.Now.AddDays(7);

            return response;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> RemoverRepository(string usuarioId)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, usuarioId);
            var update = Builders<Usuario>.Update.Set(x => x.TokenExpiracion, DateTime.UtcNow);
            await _usuariosCollection.UpdateOneAsync(filter, update);
            var update2 = Builders<Usuario>.Update.Set(x => x.TokenAcceso, null);
            await _usuariosCollection.UpdateOneAsync(filter, update2);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> ValidarRepository(string usuarioId, string token)
    {
        if (string.IsNullOrWhiteSpace(usuarioId) || string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var tokenValido = await _usuariosCollection.Find(u => u.IdUsuario == usuarioId && u.TokenAcceso == token).FirstOrDefaultAsync();
            if (tokenValido == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(tokenValido.TokenAcceso))
            {
                return false;
            }

            return !await ValidateRefreshTokenBeforeDb(tokenValido.TokenAcceso);
        }
        catch (MongoException ex)
        {
            throw new NotImplementedException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    private static Task<bool> ValidateRefreshTokenBeforeDb(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(false);
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token))
            {
                return Task.FromResult(false);
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (string.IsNullOrEmpty(expClaim) || !long.TryParse(expClaim, out var expUnixTime))
            {
                return Task.FromResult(false);
            }

            var expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTime).UtcDateTime;
            var now = DateTime.UtcNow;

            if (expDateTime <= now)
            {
                return Task.FromResult(true);
            }

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Task.FromResult(false);
            }

            var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
            return Task.FromResult<bool>(tokenType == "refresh_token");
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}