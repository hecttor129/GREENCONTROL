using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Tareas : BaseEntity
    {
        [Key]
        [Column("ID_TAREA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Parcela")]
        [Column("ID_PARCELA")]
        public int IdParcela { get; set; }

        [Column("TIPOTAREA")]
        [StringLength(20)]
        public string TipoTarea { get; set; }

        [Column("FECHATAREA")]
        public DateTime? FechaTarea { get; set; }

        [Column("URGENCIA")]
        public int? Urgencia { get; set; }

        [Column("ESTADO")]
        public string EstadoChar { get; set; } = "0";

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Column("COSTO")]
        public decimal? Costo { get; set; }
    }

}

