using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Rentabilidad 
    {
        [Key]
        [Column("ID_RENTABILIDAD")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Parcela")]
        [Column("ID_PARCELA")]
        public int IdParcela { get; set; }

        [Column("PRECIOVENTAUNITARIO")]
        public decimal? PrecioVentaUnitario { get; set; }

        [Column("COSTOTOTALPRODUCCION")]
        public decimal? CostoTotalProduccion { get; set; }

        [Column("CANTIDADCOSECHADA")]
        public decimal? CantidadCosechada { get; set; }

        [Column("INGRESOTOTAL")]
        public decimal? IngresoTotal { get; set; }

        [Column("RENTABILIDADPORCENTUAL")]
        public decimal? RentabilidadPorcentual { get; set; }

        [Column("CALIDADCOSECHADA")]
        [StringLength(15)]
        public string CalidadCosechada { get; set; }

    }
}
