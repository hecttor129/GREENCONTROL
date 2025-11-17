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
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Column("TIPO")]
        [StringLength(15)]
        public string Tipo { get; set; }

        [Column("MENSAJE")]
        public string Mensaje { get; set; }

        [Column("TITULO")]
        [StringLength(20)]
        public string Titulo { get; set; }

        [Column("FECHAENVIO")]
        public DateTime? FechaEnvio { get; set; } = DateTime.Now;

        [Required]
        [Column("LEIDO")]
        [StringLength(1)]
        public string Leido { get; set; } = "0";

        [Required]
        [Column("ESTADO")]
        [StringLength(1)]
        public string Estado { get; set; } = "1";
    }
}


