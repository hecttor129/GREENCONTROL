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
        public int Id { get; set; }

        [Column("IDPARCELA")]
        [Required]
        public int IdParcela { get; set; }

        [Column("FECHAGASTOS")]   
        public DateTime? Fecha { get; set; }

        [Column("RECURRENCIA")]
        public int? Recurrencia { get; set; }

        [Column("TIPO")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("DESCRIPCION")]   
        public string Descripcion { get; set; }

        [Column("MONTO")]
        public decimal Monto { get; set; }

        [ForeignKey("IDPARCELA")]
        public virtual Parcela Parcela { get; set; }

       
    }
}



