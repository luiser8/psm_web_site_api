using psm_web_site_api_project.Entities;

namespace psm_web_site_api_project.Repository.Auditorias;
    public interface IAuditoriasRepository
    {
        Task<List<Auditoria>> SelectAuditoriasPorUsuarioIdRepository(string idUsuario);
        Task<bool> PostAuditoriasRepository(Auditoria auditoria);
        Task<bool> DeleteAuditoriasRepository(string idAuditoria);
    }
