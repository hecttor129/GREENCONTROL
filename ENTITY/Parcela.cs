using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("PARCELA")]
    public class Parcela
    {
        [Key]
        [Column("IDPARCELA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdParcela { get; set; }

        [Required]
        [ForeignKey("Finca")]
        [Column("IDFINCA")]
        public int IdFinca { get; set; }

        [Required]
        [ForeignKey("Cultivo")]
        [Column("IDCULTIVO")]
        public int IdCultivo { get; set; }

        [Column("AREACALCULADA")]
        public decimal? AreaCalculada { get; set; }

        [Column("NOMBRE")]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Column("POLIGONOS")]
        public string Poligonos { get; set; }  

        public virtual Finca Finca { get; set; }
        public virtual Cultivo Cultivo { get; set; }

        [Column("PHSUELO")]
        public decimal? PhSuelo { get; set; }

        [Column("TIPOSUELO")]
        [StringLength(30)]
        public string TipoSuelo { get; set; }

        public Parcela()
        {
        }
    }
}

