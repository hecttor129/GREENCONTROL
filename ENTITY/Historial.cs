using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("HISTORIAL")]
    public class Historial
    {
        [Key]
        [Column("IDHISTORIAL")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHistorial { get; set; }

        [Required]
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Column("FECHASIEMBRA")]
        public DateTime? FechaSiembra { get; set; }

        [Column("FECHACOSECHA")]
        public DateTime? FechaCosecha { get; set; }

        [Column("DIASGERMINACION")]
        public int? DiasGerminacion { get; set; }

        [Column("DIASFLORACION")]
        public int? DiasFloracion { get; set; }

        [Required]
        [Column("DURACIONCICLO")]
        public int DuracionCiclo { get; set; }

        [Column("CALIDADCOSECHADA")]
        public int? CalidadCosechada { get; set; }

        [Column("CANTIDADCOSECHADA", TypeName = "NUMBER(10,2)")]
        public decimal? CantidadCosechada { get; set; }

        [Required]
        [Column("NOMBRECULTIVO")]
        [StringLength(40)]
        public string NombreCultivo { get; set; }

        [Required]
        [Column("NOMBREPARCELA")]
        [StringLength(40)]
        public string NombreParcela { get; set; }

        [Column("TIPOSUELO")]
        [StringLength(30)]
        public string? TipoSuelo { get; set; }

        [Column("PHSUELO", TypeName = "NUMBER(4,2)")]
        public decimal? PhSuelo { get; set; }

        [Column("ESTADO", TypeName = "CHAR(1)")]
        public char Estado { get; set; } = '1';

        [Column("COSTOTOTALPRODUCCION", TypeName = "NUMBER(12,2)")]
        public decimal? CostoTotalProduccion { get; set; }

        [Column("INGRESOTOTAL", TypeName = "NUMBER(12,2)")]
        public decimal? IngresoTotal { get; set; }

        [Column("RENTABILIDADFINAL", TypeName = "NUMBER(6,2)")]
        public decimal? RentabilidadFinal { get; set; }

        [Column("FECHASNAPSHOT")]
        public DateTime? FechaSnapshot { get; set; }

        // Relación con Parcela
        [ForeignKey("IDPARCELA")]
        public virtual Parcela? Parcela { get; set; }

        public Historial() { }
    }
}


