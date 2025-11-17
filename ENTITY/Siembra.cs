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
        [Column("IDPARCELA")]
        public int IdParcela { get; set; }

        [Required]
        [Column("IDCULTIVO")]
        public int IdCultivo { get; set; }

        [Required]
        [Column("FECHASIEMBRA")]
        public DateTime FechaSiembra { get; set; }

        [Column("FECHACOSECHA")]
        public DateTime? FechaCosecha { get; set; }

        [Required]
        [Column("ESTADO")]
        [StringLength(1)]
        public string Estado { get; set; } = "1";

        [ForeignKey("IdParcela")]
        public virtual Parcela Parcela { get; set; }

        [ForeignKey("IdCultivo")]
        public virtual Cultivo Cultivo { get; set; }
    }
}

