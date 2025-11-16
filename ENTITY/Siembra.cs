using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    [Table("SIEMBRA")]
    public class Siembra
    {
        [Key]
        [Column("IDSIEMBRA")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSiembra { get; set; }

        [Required]
        [ForeignKey("Parcela")]
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Required]
        [ForeignKey("Cultivo")]
        [Column("IDCULTIVO")]
        public int IdCultivo { get; set; }

        [Column("ESTADO")]
        [StringLength(1)]
        public string EstadoChar { get; set; } = "1";

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }

        [Required]
        [Column("FECHASIEMBRA")]
        public DateTime FechaSiembra { get; set; }

        [Column("PORCENTAJEDESARROLLO")]
        public decimal? PorcentajeDesarrollo { get; set; } = 0;

        [Column("FECHAGERMINACION")]
        public DateTime? FechaGerminacion { get; set; }

        [Column("FECHAFLORACION")]
        public DateTime? FechaFloracion { get; set; }

        [Column("FECHACOSECHA")]
        public DateTime? FechaCosecha { get; set; }

        [Column("GERMINACIONCONFIRMADA")]
        [StringLength(1)]
        public string GerminacionConfirmadaChar { get; set; } = "0";

        [NotMapped]
        public bool GerminacionConfirmada
        {
            get => GerminacionConfirmadaChar == "1";
            set => GerminacionConfirmadaChar = value ? "1" : "0";
        }

        [Column("FLORACIONCONFIRMADA")]
        [StringLength(1)]
        public string FloracionConfirmadaChar { get; set; } = "0";

        [NotMapped]
        public bool FloracionConfirmada
        {
            get => FloracionConfirmadaChar == "1";
            set => FloracionConfirmadaChar = value ? "1" : "0";
        }

        [Column("COSECHACONFIRMADA")]
        [StringLength(1)]
        public string CosechaConfirmadaChar { get; set; } = "0";

        [NotMapped]
        public bool CosechaConfirmada
        {
            get => CosechaConfirmadaChar == "1";
            set => CosechaConfirmadaChar = value ? "1" : "0";
        }

        // Navegación
        public virtual Parcela Parcela { get; set; }
        public virtual Cultivo Cultivo { get; set; }
    }
}

