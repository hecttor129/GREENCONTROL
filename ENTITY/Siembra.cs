using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Siembra : BaseEntity
    {
        [Key]
        [Column("ID_SIEMBRA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Parcela")]
        [Column("ID_PARCELA")]
        public int IdParcela { get; set; }

        [ForeignKey("Cultivo")]
        [Column("ID_CULTIVO")]
        public int IdCultivo { get; set; }

        [Column("ESTADO")]
        [StringLength(20)]
        public string Estado { get; set; }

        [Column("PORCENTADEDESARROLLO")]
        public decimal? PorcentajeDesarrollo { get; set; }

        [Column("FECHASIEMBRA")]
        public DateTime FechaSiembra { get; set; }

        [Column("FECHAGERMINACION")]
        public DateTime? FechaGerminacion { get; set; }

        [Column("FECHAFLORACION")]
        public DateTime? FechaFloracion { get; set; }

        [Column("FECHACOSECHAESTIMADA")]
        public DateTime? FechaCosechaEstimada { get; set; }

        [Column("FECHACOSECHAREAL")]
        public DateTime? FechaCosechaReal { get; set; }
    }
}

