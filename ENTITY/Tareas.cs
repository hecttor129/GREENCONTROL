using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    [Table("TAREAS")]
    public class Tareas
    {
        [Key]
        [Column("IDTAREA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTarea { get; set; }

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
        public string EstadoChar { get; set; } = "0";

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Column("URGENCIA")]
        public int? Urgencia { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        public virtual Parcela Parcela { get; set; }
    }

}

