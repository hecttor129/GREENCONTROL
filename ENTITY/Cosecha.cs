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

        [Column("idSiembra")]
        [Required]
        public int IdSiembra { get; set; }

        [Column("calidadCosechada")]
        public int? CalidadCosechada { get; set; }

        [Column("cantidadCosechada")]
        public decimal? CantidadCosechada { get; set; }

        [Column("PrecioVenta")]
        public decimal? PrecioVenta { get; set; }

        [Column("estado", TypeName = "CHAR(1)")]
        [Required]
        public char Estado { get; set; } = '1'; // Activo por defecto

        // Relación
        [ForeignKey("IdSiembra")]
        public virtual Siembra? Siembra { get; set; }
    }
}


