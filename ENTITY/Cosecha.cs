using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("COSECHA")]
    public class Cosecha : BaseEntity
    {
        [Key]
        [Column("id_cosecha")]
        public int Id { get; set; }

        [Column("id_siembra")]
        [Required]
        public int IdSiembra { get; set; }

        [Column("FechaCosechaReal")]
        [Required]
        public DateTime FechaCosechaReal { get; set; }

        [Column("FechaCosechaEstimada")]
        public DateTime? FechaCosechaEstimada { get; set; }

        [Column("FechaGerminacion")]
        public DateTime? FechaGerminacion { get; set; }

        [Column("FechaFloracion")]
        public DateTime? FechaFloracion { get; set; }

        [Column("FechaSiembra")]
        [Required]
        public DateTime FechaSiembra { get; set; }

        [Column("PorcentajeDesarrollo")]
        public decimal? PorcentajeDesarrollo { get; set; }

        [Column("Estado")]
        [StringLength(1)]
        [Required]
        public string Estado { get; set; } = "1"; // Por defecto '1'

        // 🔗 Relación opcional con Siembra (por FK)
        [ForeignKey("IdSiembra")]
        public Siembra Siembra { get; set; }

        //// 🔹 Constructores
        //public Cosecha() { }

        //public Cosecha(int id, int idSiembra, DateTime fechaCosechaReal, DateTime? fechaCosechaEstimada,
        //               DateTime? fechaGerminacion, DateTime? fechaFloracion, DateTime fechaSiembra,
        //               decimal? porcentajeDesarrollo, string estado)
        //{
        //    Id = id;
        //    IdSiembra = idSiembra;
        //    FechaCosechaReal = fechaCosechaReal;
        //    FechaCosechaEstimada = fechaCosechaEstimada;
        //    FechaGerminacion = fechaGerminacion;
        //    FechaFloracion = fechaFloracion;
        //    FechaSiembra = fechaSiembra;
        //    PorcentajeDesarrollo = porcentajeDesarrollo;
        //    Estado = estado;
        //}

        //public Cosecha(int idSiembra, DateTime fechaCosechaReal, DateTime? fechaCosechaEstimada,
        //               DateTime? fechaGerminacion, DateTime? fechaFloracion, DateTime fechaSiembra,
        //               decimal? porcentajeDesarrollo, string estado)
        //{
        //    IdSiembra = idSiembra;
        //    FechaCosechaReal = fechaCosechaReal;
        //    FechaCosechaEstimada = fechaCosechaEstimada;
        //    FechaGerminacion = fechaGerminacion;
        //    FechaFloracion = fechaFloracion;
        //    FechaSiembra = fechaSiembra;
        //    PorcentajeDesarrollo = porcentajeDesarrollo;
        //    Estado = estado;
        //}
    }
}
