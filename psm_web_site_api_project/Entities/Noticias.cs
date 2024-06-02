using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Noticias
    {
        [Key]
        public int IdNoticia;
        public string? Title;
        public string? Descripcion;
        public string? Imagen;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}