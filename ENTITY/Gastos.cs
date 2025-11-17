using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("GASTOS")]
    public class Gastos
    {
        [Key]
        [Column("IDGASTO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGasto { get; set; }

        [Required]
        [Column("IDSIEMBRA")]
        public int IdSiembra { get; set; }

        [Column("FECHAGASTOS")]
        public DateTime? FechaGastos { get; set; }

        [Column("TIPO")]
        [StringLength(20)]
        public string? Tipo { get; set; }

        [Column("CONCEPTO")]
        public string? Concepto { get; set; } 

        [Column("MONTO", TypeName = "NUMBER(10,2)")]
        public decimal Monto { get; set; }

        [Column("NOTA")]
        public string? Nota { get; set; } 

        [Column("ESTADO", TypeName = "CHAR(1)")]
        [Required]
        public char Estado { get; set; } = '1';

        // Relación con Siembra
        [ForeignKey("IDSIEMBRA")]
        public virtual Siembra? Siembra { get; set; }
    }
}



