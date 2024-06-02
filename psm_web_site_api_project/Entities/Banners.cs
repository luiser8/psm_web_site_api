using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Banners
    {
        [Key]
        public int IdBanner;
        public string? Titulo;
        public string? Descripcion;
        public string? FullDescripcion;
        public string? Link;
        public string? Imagen;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}