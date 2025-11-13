using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("CULTIVO")]
    public class Cultivo 
    {
        [Key]
        [Column("id_cultivo")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("Nombre")]
        [Required]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Column("Variedad")]
        [StringLength(30)]
        public string Variedad { get; set; }

        [Column("DuracionCiclo_Fecha1")]
        public int? DuracionCiclo_Fecha1 { get; set; }

        [Column("DuracionCiclo_Fecha2")]
        public int? DuracionCiclo_Fecha2 { get; set; }

        [Column("DiasGerminacion_Fecha1")]
        public int? DiasGerminacion_Fecha1 { get; set; }

        [Column("DiasGerminacion_Fecha2")]
        public int? DiasGerminacion_Fecha2 { get; set; }

        [Column("DiasFloracion_Fecha1")]
        public int? DiasFloracion_Fecha1 { get; set; }

        [Column("DiasFloracion_Fecha2")]
        public int? DiasFloracion_Fecha2 { get; set; }

        [Column("DiasCosecha_Fecha1")]
        public int? DiasCosecha_Fecha1 { get; set; }

        [Column("DiasCosecha_Fecha2")]
        public int? DiasCosecha_Fecha2 { get; set; }

        [Column("TipoSuelo")]
        [StringLength(30)]
        public string TipoSuelo { get; set; }

        [Column("TemperaturaOptima")]
        public decimal? TemperaturaOptima { get; set; }

        [Column("PhSuelo")]
        public decimal? PhSuelo { get; set; }

        [Column("HumedadOptima")]
        public decimal? HumedadOptima { get; set; }

        [Column("Descripcion")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        //// 🔹 Constructores
        ////public Cultivo() { }

        ////public Cultivo(int id, string nombre, string variedad, int? duracionCiclo_Fecha1, int? duracionCiclo_Fecha2,
        ////               int? diasGerminacion_Fecha1, int? diasGerminacion_Fecha2, int? diasFloracion_Fecha1,
        ////               int? diasFloracion_Fecha2, int? diasCosecha_Fecha1, int? diasCosecha_Fecha2,
        ////               string tipoSuelo, decimal? temperaturaOptima, decimal? phSuelo,
        ////               decimal? humedadOptima, string descripcion)
        ////{
        ////    Id = id;
        ////    Nombre = nombre;
        ////    Variedad = variedad;
        ////    DuracionCiclo_Fecha1 = duracionCiclo_Fecha1;
        ////    DuracionCiclo_Fecha2 = duracionCiclo_Fecha2;
        ////    DiasGerminacion_Fecha1 = diasGerminacion_Fecha1;
        ////    DiasGerminacion_Fecha2 = diasGerminacion_Fecha2;
        ////    DiasFloracion_Fecha1 = diasFloracion_Fecha1;
        ////    DiasFloracion_Fecha2 = diasFloracion_Fecha2;
        ////    DiasCosecha_Fecha1 = diasCosecha_Fecha1;
        ////    DiasCosecha_Fecha2 = diasCosecha_Fecha2;
        ////    TipoSuelo = tipoSuelo;
        ////    TemperaturaOptima = temperaturaOptima;
        ////    PhSuelo = phSuelo;
        ////    HumedadOptima = humedadOptima;
        ////    Descripcion = descripcion;
        ////}

        ////public Cultivo(string nombre, string variedad, int? duracionCiclo_Fecha1, int? duracionCiclo_Fecha2,
        ////               int? diasGerminacion_Fecha1, int? diasGerminacion_Fecha2, int? diasFloracion_Fecha1,
        ////               int? diasFloracion_Fecha2, int? diasCosecha_Fecha1, int? diasCosecha_Fecha2,
        ////               string tipoSuelo, decimal? temperaturaOptima, decimal? phSuelo,
        ////               decimal? humedadOptima, string descripcion)
        ////{
        ////    Nombre = nombre;
        ////    Variedad = variedad;
        ////    DuracionCiclo_Fecha1 = duracionCiclo_Fecha1;
        ////    DuracionCiclo_Fecha2 = duracionCiclo_Fecha2;
        ////    DiasGerminacion_Fecha1 = diasGerminacion_Fecha1;
        ////    DiasGerminacion_Fecha2 = diasGerminacion_Fecha2;
        ////    DiasFloracion_Fecha1 = diasFloracion_Fecha1;
        ////    DiasFloracion_Fecha2 = diasFloracion_Fecha2;
        ////    DiasCosecha_Fecha1 = diasCosecha_Fecha1;
        ////    DiasCosecha_Fecha2 = diasCosecha_Fecha2;
        ////    TipoSuelo = tipoSuelo;
        ////    TemperaturaOptima = temperaturaOptima;
        ////    PhSuelo = phSuelo;
        ////    HumedadOptima = humedadOptima;
        ////    Descripcion = descripcion;
        ////}
    }
}

