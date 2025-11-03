using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models
{
    public class Genero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Nombre { get; set; }

        [MaxLength(300)]
        public string? Descripcion { get; set; }

              public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
    }
}
