using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Carreras
    {
        [Key]
        public int IdCarrera;
        public int IdMasInformacion;
        public string? Nombre;
        public string? Imagen;
        public string? LinkDetalle;
        public string? Descripcion;
        public string? FullDescripcion;
        public string? Pensum;
        public DateTime FechaCreacion;
        public bool Activo;
        public virtual CarrerasRelacionadas CarrerasRelacionadas { get; set; } = null!;
    }
}