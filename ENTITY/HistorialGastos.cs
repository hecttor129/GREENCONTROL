using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("HISTORIALGASTOS")]
    public class HistorialGastos
    {
        [Key]
        [Column("IDHISTORIALGASTO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHistorialGasto { get; set; }

        [Required]
        [ForeignKey("Parcela")]
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Column("FECHAGASTO")]
        public DateTime? FechaGasto { get; set; }

        [Column("RECURRENCIA")]
        public int? Recurrencia { get; set; }

        [Column("TIPO")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } // CLOB

        [Column("MONTO")]
        public decimal? Monto { get; set; }

        // Navegación
        public virtual Parcela Parcela { get; set; }
    }
}

