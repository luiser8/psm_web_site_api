using System.ComponentModel.DataAnnotations;

namespace psmwebsite.Entities
{
    public class Saia
    {
        [Key]
        public int IdSaia;
        public string? Nombre;
        public string? Imagen;
        public string? LinkDetalle;
        public string? URLSitio;
        public string? Descripcion;
        public string? FullDescripcion;
        public DateTime FechaCreacion;
        public bool Activo;
    }
}