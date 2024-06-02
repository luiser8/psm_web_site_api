using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class CarrerasRelacionadas
    {
        [Key]
        public int IdCarreraRelacionadas;
        public string? Nombre;
        public DateTime FechaCreacion;
        public bool Activo;
        public virtual ICollection<Carreras> Carreras { get; set; } = new List<Carreras>();
    }
}