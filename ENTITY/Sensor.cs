using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Sensor 
    {
        [Key]
        [Column("ID_SENSOR")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Parcela")]
        [Column("ID_PARCELA")]
        public int IdParcela { get; set; }

        [Column("ESTADO")]
        public string EstadoChar { get; set; } = "1";

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Column("FRECUENCIALECTURA")]
        public int? FrecuenciaLectura { get; set; }

        [Column("HUMEDADACTUAL")]
        public decimal? HumedadActual { get; set; }

    }
}
