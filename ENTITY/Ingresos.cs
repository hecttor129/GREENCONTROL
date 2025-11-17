using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    [Table("INGRESOS")]
    public class Ingresos
    {
        [Key]
        [Column("IDINGRESOS")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdIngresos { get; set; }

        [Required]
        [Column("IDSIEMBRA")]
        public int IdSiembra { get; set; }

        [ForeignKey("IdSiembra")]
        public virtual Siembra Siembra { get; set; }

        [Column("FECHAINGRESOS")]
        public DateTime? FechaIngresos { get; set; }

        [Column("TIPO")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("CONCEPTO")]
        public string Concepto { get; set; }

        [Column("MONTO", TypeName = "NUMBER(10,2)")]
        public decimal? Monto { get; set; }

        [Column("NOTA")]
        public string Nota { get; set; }

        [Required]
        [Column("ESTADO")]
        [StringLength(1)]
        public string Estado { get; set; } = "1";
    }
}
