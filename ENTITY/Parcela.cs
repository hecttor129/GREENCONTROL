using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("PARCELA")]
    public class Parcela 
    {
        [Key]
        [Column("id_parcela")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_usuario")]
        [Required]
        public int IdUsuario { get; set; }

        [Column("nombre")]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Column("ubicacion")]
        [StringLength(30)]
        public string Ubicacion { get; set; }

        [Column("area")]
        public double Area { get; set; }

        // 🔗 Relación con Usuario
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        // 🔗 Relación con Insumo (1 Parcela → muchos Insumos)
        public virtual ICollection<Insumo> Insumos { get; set; }

        public Parcela()
        {
            Insumos = new HashSet<Insumo>();
        }

        //public Parcela(int idUsuario, string nombre, string ubicacion, double area)
        //{
        //    IdUsuario = idUsuario;
        //    Nombre = nombre;
        //    Ubicacion = ubicacion;
        //    Area = area;
        //    Insumos = new HashSet<Insumo>();
        //}
    }
}

