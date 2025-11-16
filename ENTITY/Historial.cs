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

        [Column("IDPARCELA")]
        public int? IdParcela { get; set; }

        [Column("FECHACOSECHA")]
        public DateTime? FechaCosecha { get; set; }

        [Column("FECHASIEMBRA")]
        public DateTime? FechaSiembra { get; set; }

        [Column("ETAPAACTUAL")]
        [StringLength(20)]
        public string EtapaActual { get; set; }

        [Column("CALIDADCOSECHADA")]
        public int? CalidadCosechada { get; set; }

        [Column("CANTIDADCOSECHADA")]
        public decimal? CantidadCosechada { get; set; }

        [Column("DURACIONCICLO")]
        public int? DuracionCiclo { get; set; }

        [Column("NOMBRECULTIVO")]
        [StringLength(20)]
        public string NombreCultivo { get; set; }

        [Column("NOMBREPARCELA")]
        [StringLength(20)]
        public string NombreParcela { get; set; }

        [Column("TIPOSUELO")]
        [StringLength(25)]
        public string TipoSuelo { get; set; }

        [Column("PHSUELO")]
        public decimal? PhSuelo { get; set; }

        [Column("ESTADO")]
        [StringLength(1)]
        public string EstadoChar { get; set; }

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Column("COSTOTOTALPRODUCCION")]
        public decimal? CostoTotalProduccion { get; set; }

        [Column("INGRESOTOTAL")]
        public decimal? IngresoTotal { get; set; }

        [Column("RENTABILIDADFINAL")]
        public decimal? RentabilidadFinal { get; set; }

        [Column("FECHASNAPSHOT")]
        public DateTime? FechaSnapshot { get; set; }

        // Nota: Historial NO tiene ForeignKey a Parcela (sobrevive por sí solo)
        // Solo guarda el IdParcela como dato histórico

        public Historial() { }
    }
}

