using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    public class Usuario : BaseEntity
    {
        [Key]
        [Column("ID_USUARIO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("NOMBRE")]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Required]
        [Column("EMAIL")]
        [StringLength(30)]
        public string Email { get; set; }

        [Required]
        [Column("PASSWORD")]
        [StringLength(15)]
        public string Password { get; set; }

        [Column("TELEFONO")]
        [StringLength(10)]
        public string Telefono { get; set; }

    }
}
