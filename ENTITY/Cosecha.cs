using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("COSECHA")]
    public class Cosecha
    {
        [Key]
        [Column("ID_COSECHA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("ID_SIEMBRA")]
        [Required]
        public int IdSiembra { get; set; }

        [Column("ESTADO")]
        [StringLength(1)]
        public string Estado { get; set; } = "0";

        [Column("CALIDAD")]
        public int? Calidad { get; set; } = 1;

        [Column("CANTIDAD")]
        public decimal? Cantidad { get; set; }

        // 🔗 Relación opcional con Siembra
        [ForeignKey("IdSiembra")]
        public Siembra? Siembra { get; set; }
    }
}

