using Microsoft.Extensions.Options;
using MongoDB.Driver;
using psm_web_site_api_project.Config;
using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Usuarios;
public class UsuariosRepository : IUsuariosRepository
{
    private readonly IMongoCollection<Usuario> _usuariosCollection;

    public UsuariosRepository(IOptions<ConfigDB> options, IMongoClient mongoClient)
    {
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

    public async Task<bool> PutUsuariosRepository(string idUsuario, Usuario usuario)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, idUsuario);
            var response = await _usuariosCollection.ReplaceOneAsync(filter, usuario);
            return response.IsModifiedCountAvailable;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> SetStatusUsuariosRepository(string idUsuario, bool status)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, idUsuario);
            var update = Builders<Usuario>.Update.Set(x => x.Activo, status);
            var response = await _usuariosCollection.UpdateOneAsync(filter, update);
            return response.IsModifiedCountAvailable;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }

    public async Task<bool> DeleteUsuariosRepository(string idUsuario)
    {
        try
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.IdUsuario, idUsuario);
            var response = await _usuariosCollection.DeleteOneAsync(filter);
            return response.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw new NotImplementedException(ex.Message);
        }
    }
}