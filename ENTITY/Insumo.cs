using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("INSUMO")]
    public class Insumo : BaseEntity
    {
        [Key]
        [Column("id_insumo")]
        public int Id { get; set; }

        [Column("id_parcela")]
        [Required]
        public int IdParcela { get; set; }

        [Column("unidad")]
        public int Unidad { get; set; }

        [Column("tipo")]
        [StringLength(20)]
        public string Tipo { get; set; }

        [Column("costo_unitario")]
        public float CostoUnitario { get; set; }

        // 🔗 Relación con Parcela
        [ForeignKey("IdParcela")]
        public virtual Parcela Parcela { get; set; }

        public Insumo() { }

        //public Insumo(int id, int idParcela, int unidad, string tipo, float costoUnitario)
        //{
        //    Id = id;
        //    IdParcela = idParcela;
        //    Unidad = unidad;
        //    Tipo = tipo;
        //    CostoUnitario = costoUnitario;
        //}

        //public Insumo(int idParcela, int unidad, string tipo, float costoUnitario)
        //{
        //    IdParcela = idParcela;
        //    Unidad = unidad;
        //    Tipo = tipo;
        //    CostoUnitario = costoUnitario;
        //}
    }
}


