using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Institucion
    {
        [Key]
        public int IdInstitucion;
        public string? Descripcion;
        public string? Imagen;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}