using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlmacenSQLiteEntities
{
    [Table("Mantenimiento")]
    public class Mantenimiento
    {
        public Mantenimiento()
        {
            ReporteMantenimientos = new HashSet<ReporteMantenimiento>();
        }

        [Key]
        [Column(TypeName = "int")]
        public int MantenimientoId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo Nombre debe tener entre 2 y 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Column(TypeName = "ntext")]
        public string Descripcion { get; set; } = null!;

        [InverseProperty("Mantenimiento")]
        public virtual ICollection<ReporteMantenimiento>? ReporteMantenimientos { get; set; }
    }

}
