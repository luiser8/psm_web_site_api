using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class AcercaDe
    {
        [Key]
        public int IdAcercaDe;
        public string? Titulo;
        public string? Descripcion;
        public string? FullDescripcion;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}