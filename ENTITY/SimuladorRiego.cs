using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class SimuladorRiego 
    {
        [Key]
        [Column("ID_RIEGO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Parcela")]
        [Column("ID_PARCELA")]
        public int IdParcela { get; set; }

        [Column("ESTADORIEGO")]
        [StringLength(15)]
        public string EstadoRiego { get; set; }

        [Column("UMBRALACTIVACION")]
        public decimal? UmbralActivacion { get; set; }

    }
}
