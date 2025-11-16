using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENTITY
{
    [Table("CULTIVO")]
    public class Cultivo
    {
        [Key]
        [Column("IDCULTIVO")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("NOMBRE")]
        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }

        [Column("VARIEDAD")]
        [StringLength(20)]
        public string Variedad { get; set; }

        [Column("DURACIONCICLO_FECHA1")]
        public int? DuracionCiclo_Fecha1 { get; set; }

        [Column("DURACIONCICLO_FECHA2")]
        public int? DuracionCiclo_Fecha2 { get; set; }

        [Column("DIASGERMINACION_FECHA1")]
        public int? DiasGerminacion_Fecha1 { get; set; }

        [Column("DIASGERMINACION_FECHA2")]
        public int? DiasGerminacion_Fecha2 { get; set; }

        [Column("DIASFLORACION_FECHA1")]
        public int? DiasFloracion_Fecha1 { get; set; }

        [Column("DIASFLORACION_FECHA2")]
        public int? DiasFloracion_Fecha2 { get; set; }

        [Column("DIASCOSECHA_FECHA1")]
        public int? DiasCosecha_Fecha1 { get; set; }

        [Column("DIASCOSECHA_FECHA2")]
        public int? DiasCosecha_Fecha2 { get; set; }

        [Column("TEMPERATURAOPTIMA")]
        public decimal? TemperaturaOptima { get; set; }

        [Column("TIPOSUELO")]
        [StringLength(20)]
        public string TipoSuelo { get; set; }

        [Column("PHSUELO")]
        public decimal? PhSuelo { get; set; }

        [Column("HUMEDADOPTIMA")]
        public decimal? HumedadOptima { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }  
    }
}



