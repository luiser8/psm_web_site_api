using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Carousel
    {
        [Key]
        public int IdCarousel;
        public string? Descripcion;
        public string? Imagen;
        public string? Link;
        public string? Title;
        public string? Target;
        public string? Iframe;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}