
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models
{
    public class ActorPelicula
    {
        [Key]
        public int Id { get; set; }

        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }
}
