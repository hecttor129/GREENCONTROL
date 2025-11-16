using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    [Table("PREDICCIONESSIEMBRA")]
    public class PrediccionesSiembra
    {
        [Key]
        [Column("IDPREDICCION")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPrediccion { get; set; }

        [Required]
        [Column("IDSIEMBRA")]
        public int IdSiembra { get; set; }

        [Column("FECHACALCULO")]
        public DateTime? FechaCalculo { get; set; }

        [Column("DIASGERMINACIONESTIMADO")]
        public int? DiasGerminacionEstimado { get; set; }

        [Column("DIASFLORACIONESTIMADO")]
        public int? DiasFloracionEstimado { get; set; }

        [Column("DIASCOSECHAESTIMADO")]
        public int? DiasCosechaEstimado { get; set; }

        [Column("DURACIONCICLOESTIMADO")]
        public int? DuracionCicloEstimado { get; set; }

        [Column("CALIDADESPERADA")]
        public int? CalidadEsperada { get; set; }

        [Column("CANTIDADESTIMADA")]
        public decimal? CantidadEstimada { get; set; }

        [Column("RENTABILIDADESPERADA")]
        public decimal? RentabilidadEsperada { get; set; }

        // Navegación a Siembra si la usas
        public Siembra Siembra { get; set; }
    }
}
