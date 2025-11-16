using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("FINCA")]
    public class Finca
    {
        [Key]
        [Column("IDFINCA")]                    
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("IDUSUARIO")]                 
        [Required]
        public int IdUsuario { get; set; }

        [Column("AREACALCULADA")]
        public double? AreaCalculada { get; set; }

        [Column("POLIGONO")]
        public string? Poligono { get; set; }  

        // 🔗 Relación con Usuario
        [ForeignKey("IDUSUARIO")]
        public virtual Usuario? Usuario { get; set; }

        // 🔗 Relación: una finca tiene muchas parcelas
        public virtual ICollection<Parcela> Parcelas { get; set; }

        public Finca()
        {
            Parcelas = new HashSet<Parcela>();
        }
    }
}


