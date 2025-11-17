using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("CULTIVO")]
    public class Cultivo
    {
        [Key]
        [Column("IDCULTIVO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCultivo { get; set; }

        [Required]
        [Column("NOMBRE")]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Column("VARIEDAD")]
        [StringLength(20)]
        public string Variedad { get; set; }

        [Column("DURACIONCICLO")]
        public int? DuracionCiclo { get; set; }

        [Required]
        [Column("FECHASIEMBRA")]
        public DateTime FechaSiembra { get; set; }

        [Column("FECHAGERMINACION")]
        public DateTime? FechaGerminacion { get; set; }

        [Column("FECHAFLORACION")]
        public DateTime? FechaFloracion { get; set; }

        [Column("FECHACOSECHA")]
        public DateTime? FechaCosecha { get; set; }

        [Column("ESTADO")]
        [StringLength(1)]
        public string EstadoChar { get; set; } = "1"; // Valor por defecto

        [NotMapped]
        public bool Estado
        {
            get => EstadoChar == "1";
            set => EstadoChar = value ? "1" : "0";
        }
    }
}



