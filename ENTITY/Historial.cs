using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("HISTORIAL")]
    public class Historial 
    {
        [Key]
        [Column("id_historial")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("id_parcela")]
        [Required]
        public int IdParcela { get; set; }

        [Column("HFechaSiembra")]
        [Required]
        public DateTime HFechaSiembra { get; set; }

        [Column("HFechaCosecha")]
        public DateTime? HFechaCosecha { get; set; }

        [Column("DuracionCiclo")]
        public int? DuracionCiclo { get; set; }

        [Column("EtapaActual")]
        [StringLength(20)]
        public string EtapaActual { get; set; }

        [Column("RegistroInsumos", TypeName = "CLOB")]
        public string RegistroInsumos { get; set; }

        [Column("RegistroTareas", TypeName = "CLOB")]
        public string RegistroTareas { get; set; }

        [ForeignKey("IdParcela")]
        public Parcela Parcela { get; set; }

        // 🔹 Constructores
        //public Historial() { }

        //public Historial(int id, int idParcela, DateTime hFechaSiembra, DateTime? hFechaCosecha,
        //                 int? duracionCiclo, string etapaActual, string registroInsumos, string registroTareas)
        //{
        //    Id = id;
        //    IdParcela = idParcela;
        //    HFechaSiembra = hFechaSiembra;
        //    HFechaCosecha = hFechaCosecha;
        //    DuracionCiclo = duracionCiclo;
        //    EtapaActual = etapaActual;
        //    RegistroInsumos = registroInsumos;
        //    RegistroTareas = registroTareas;
        //}

        //public Historial(int idParcela, DateTime hFechaSiembra, DateTime? hFechaCosecha,
        //                 int? duracionCiclo, string etapaActual, string registroInsumos, string registroTareas)
        //{
        //    IdParcela = idParcela;
        //    HFechaSiembra = hFechaSiembra;
        //    HFechaCosecha = hFechaCosecha;
        //    DuracionCiclo = duracionCiclo;
        //    EtapaActual = etapaActual;
        //    RegistroInsumos = registroInsumos;
        //    RegistroTareas = registroTareas;
        //}
    }
}

