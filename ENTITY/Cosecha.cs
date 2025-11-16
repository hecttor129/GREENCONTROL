using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("COSECHA")]
    public class Cosecha
    {
        [Key]
        [Column("IDCosecha")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("idParcela")]
        [Required]
        public int IdParcela { get; set; }

        [Column("calidadCosechada")]
        public int? CalidadCosechada { get; set; }

        [Column("cantidadCosechada")]
        public decimal? CantidadCosechada { get; set; }

        [Column("PrecioVentaUnitario")]
        public decimal? PrecioVentaUnitario { get; set; }

        [ForeignKey("IdParcela")]
        public virtual Parcela? Parcela { get; set; }
    }
}


