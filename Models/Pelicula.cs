using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [StringLength(500)]
        public string Sinopsis { get; set; }

        public int DuracionMinutos { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaEstreno { get; set; }

        [MaxLength(50 * 1024 * 1024)]
        public byte[] Poster { get; set; }

        [NotMapped]
        public IFormFile? PosterFile { get; set; }

        [ForeignKey("Genero")]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }

        [ForeignKey("Director")]
        public int DirectorId { get; set; }
        public Director Director { get; set; }

        public ICollection<ActorPelicula> Actores { get; set; }
    }
    
}