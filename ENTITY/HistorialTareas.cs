using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("HISTORIALTAREAS")]
    public class HistorialTareas
    {
        [Key]
        [Column("IDHISTORIALTAREAS")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHistorialTareas { get; set; }

        [Required]
        [ForeignKey("Parcela")]
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Column("TIPO")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("FECHATAREA")]
        public DateTime? FechaTarea { get; set; }

        [Column("ESTADO")]
        [StringLength(1)]
        public string EstadoChar { get; set; }

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Column("URGENCIA")]
        public int? Urgencia { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion{ get; set; }

        // Navegación
        public virtual Parcela Parcela { get; set; }
    }
}

