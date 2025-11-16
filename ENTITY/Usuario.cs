using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [Column("PASSWORD")]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [Column("EMAIL")]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [Column("NOMBRE")]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Column("TELEFONO")]
        [StringLength(10)]
        public string Telefono { get; set; }
    }
}
