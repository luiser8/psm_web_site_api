using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Dto;
using psm_web_site_api_project.Utils.JwtUtils;
using psm_web_site_api_project.Utils.Md5utils;
using psm_web_site_api_project.Entities;
using Microsoft.Extensions.Localization;

namespace psm_web_site_api_project.Repository.Usuarios;
public class UsuariosRepository : IUsuariosRepository
{
    private readonly IMongoCollection<Usuario> _usuariosCollection;

    public UsuariosRepository(IOptions<ConfigDB> options)
    {
        var mongoClient = new MongoClient(options.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(options.Value.DatabaseName);
        _usuariosCollection = mongoDatabase.GetCollection<Usuario>("usuarios");
    }

    public async Task<List<Usuario>> SelectUsuariosRepository()
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Empty;
            return await _usuariosCollection.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Usuario> SelectUsuariosPorIdRepository(string idUsuario)
    {
        try
        {
            return await _usuariosCollection.Find(driver => driver.IdUsuario == idUsuario).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Usuario> SelectUsuariosPorCorreoRepository(string correo)
    {
        try
        {
            return await _usuariosCollection.Find(x => x.Correo == correo).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Usuario> LoginUsuarioRepository(LoginPayloadDto loginPayloadDto)
    {
        try
        {
            var response = await _usuariosCollection.Find(driver => driver.Correo == loginPayloadDto.Correo && driver.Contrasena == Md5utilsClass.GetMD5(loginPayloadDto.Contrasena)).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Usuario no encontrado");

            if (response.Activo == false)
                throw new Exception("Usuario deshabilitado");

            var newAccessToken = JwtUtils.CreateToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Roles = response.Rol, Extension = response.Extension });
            var newRefreshToken = JwtUtils.RefreshToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Roles = response.Rol, Extension = response.Extension });

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

    public async Task<Usuario> PostUsuariosRepository(Usuario usuario)
    {
        try
        {
            await _usuariosCollection.InsertOneAsync(usuario);
            return usuario;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> PutUsuariosRepository(string IdUsuario, Usuario usuario)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, IdUsuario);
            await _usuariosCollection.ReplaceOneAsync(filter, usuario);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<Usuario> RefreshTokenRepository(string actualToken)
    {
        try
        {
            var response = await _usuariosCollection.Find(driver => driver.TokenRefresco == actualToken).FirstOrDefaultAsync();

            if (response == null)
                throw new Exception("Token not found");

            if (response.Activo == false)
                throw new Exception("User status disabled");

            string newAccessToken = JwtUtils.CreateToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Roles = response.Rol, Extension = response.Extension });
            string newRefreshToken = JwtUtils.RefreshToken(new TokenDto { IdUsuario = response.IdUsuario, Correo = response.Correo, Nombres = response.Nombres, Apellidos = response.Apellidos, Roles = response.Rol, Extension = response.Extension });

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

    public async Task<bool> SetStatusUsuariosRepository(string IdUsuario, bool status)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, IdUsuario);
            var update = Builders<Usuario>.Update.Set(x => x.Activo, status);
            await _usuariosCollection.UpdateOneAsync(filter, update);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteUsuariosRepository(string IdUsuario)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, IdUsuario);
            await _usuariosCollection.DeleteOneAsync(filter);
            return true;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}