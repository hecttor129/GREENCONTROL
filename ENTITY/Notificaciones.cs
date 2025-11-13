using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("NOTIFICACIONES")]
    public class Notificaciones 
    {
        [Key]
        [Column("id_notificaciones")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_usuario")]
        [Required]
        public int IdUsuario { get; set; }

        [Column("titulo")]
        [StringLength(30)]
        public string Titulo { get; set; }

        [Column("mensaje")]
        public string Mensaje { get; set; }

        [Column("fecha_envio")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Column("tipo")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("leido")]
        public bool Leido { get; set; }

        // 🔗 Relación con Usuario
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        //public Notificaciones() { }

        //public Notificaciones(int id, int idUsuario, string titulo, string mensaje, DateTime fechaEnvio, string tipo, bool leido)
        //{
        //    Id = id;
        //    IdUsuario = idUsuario;
        //    Titulo = titulo;
        //    Mensaje = mensaje;
        //    FechaEnvio = fechaEnvio;
        //    Tipo = tipo;
        //    Leido = leido;
        //}

        //public Notificaciones(int idUsuario, string titulo, string mensaje, string tipo)
        //{
        //    IdUsuario = idUsuario;
        //    Titulo = titulo;
        //    Mensaje = mensaje;
        //    Tipo = tipo;
        //    Leido = false;
        //}
    }
}

