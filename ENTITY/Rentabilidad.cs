using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    [Table("RENTABILIDAD")]
    public class Rentabilidad
    {
        [Key]
        [Column("IDRENTABILIDAD")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRentabilidad { get; set; }

        [Required]
        [ForeignKey("Parcela")]
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Column("INGRESOTOTAL")]
        public decimal? IngresoTotal { get; set; }

        [Column("COSTOTOTALPRODUCCION")]
        public decimal? CostoTotalProduccion { get; set; }

        [Column("RENTABILIDADPORCENTUAL")]
        public decimal? RentabilidadPorcentual { get; set; }

        public virtual Parcela Parcela { get; set; }
    }
}
