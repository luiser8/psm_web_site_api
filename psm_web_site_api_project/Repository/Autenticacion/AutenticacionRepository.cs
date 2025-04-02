using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Utils.JwtUtils;
using psm_web_site_api_project.Utils.Md5utils;
using psm_web_site_api_project.Entities;

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

    public async Task<Usuario> SessionRepository(LoginPayloadDto loginPayloadDto)
    {
        try
        {
            var response = await _usuariosCollection.Find(driver => driver.Correo == loginPayloadDto.Correo && driver.Contrasena == Md5utilsClass.GetMD5(loginPayloadDto.Contrasena ?? string.Empty)).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Usuario no encontrado");

            if (response.Activo == false)
                throw new Exception("Usuario deshabilitado");

            var newAccessToken = JwtUtils.CreateToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });
            var newRefreshToken = JwtUtils.RefreshToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });

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

    public async Task<Usuario> RefrescoRepository(string actualToken)
    {
        try
        {
            var response = await _usuariosCollection.Find(driver => driver.TokenRefresco == actualToken).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Token not found");

            if (response.Activo == false)
                throw new Exception("User status disabled");

            string newAccessToken = JwtUtils.CreateToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });
            string newRefreshToken = JwtUtils.RefreshToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Rol = response.Rol, Extension = response.Extension });

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
            var update3 = Builders<Usuario>.Update.Set(x => x.TokenRefresco, null);
            await _usuariosCollection.UpdateOneAsync(filter, update3);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> ValidarRepository(string usuarioId, string token)
    {
        var auth = await _usuariosCollection.Find(u => u.IdUsuario == usuarioId).FirstOrDefaultAsync();
        return auth?.TokenAcceso == token;
    }
}