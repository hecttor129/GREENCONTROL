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
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Column("AREA", TypeName = "NUMBER(10,2)")]
        public decimal? Area { get; set; }

        [Column("NOMBRE")]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Column("PHSUELO", TypeName = "NUMBER(4,2)")]
        public decimal? PhSuelo { get; set; }

        [Column("TIPOSUELO")]
        [StringLength(30)]
        public string TipoSuelo { get; set; }

        [Required]
        [Column("ESTADO")]
        [StringLength(1)]
        public string Estado { get; set; } = "1";
    }
}

