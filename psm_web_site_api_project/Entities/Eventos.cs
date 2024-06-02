using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Eventos
    {
        [Key]
        public int IdEvento;
        public string? Nombre;
        public string? Imagen;
        public string? LinkDetalle;
        public string? Descripcion;
        public DateTime FechaCreacion;
        public bool Activo;
        public virtual ICollection<DetallesEventos> DetallesEventos { get; set; } = new List<DetallesEventos>();
    }
}