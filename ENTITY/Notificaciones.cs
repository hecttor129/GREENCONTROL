using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("NOTIFICACIONES")]
    public class Notificaciones
    {
        [Key]
        [Column("IDNOTIFICACION")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdNotificacion { get; set; }

        [Required]
        [ForeignKey("Usuario")]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [ForeignKey("Parcela")]
        [Column("IDPARCELA")]
        public int? IdParcela { get; set; }

        [Column("TIPO")]
        [StringLength(15)]
        public string Tipo { get; set; }

        [Column("MENSAJE")]
        public string Mensaje { get; set; }  // CLOB → string

        [Column("TITULO")]
        [StringLength(20)]
        public string Titulo { get; set; }

        [Column("FECHAENVIO")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Column("LEIDO")]
        [StringLength(1)]
        public string LeidoChar { get; set; } = "0";

        [NotMapped]
        public bool Leido
        {
            get => LeidoChar == "1";
            set => LeidoChar = value ? "1" : "0";
        }

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Parcela Parcela { get; set; }
    }
}


