using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class DetallesEventos
    {
        [Key]
        public int IdDetalleEvento;
        public int IdEvento;
        public string? Descripcion;
        public DateTime FechaCreacion;
        public bool Activo;
        public virtual Eventos? Eventos { get; set; }
    }
}