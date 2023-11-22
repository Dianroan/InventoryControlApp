using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlmacenSQLiteEntities
{
    [Table("Modelo")]
    public class Modelo
    {
        public Modelo()
        {
            Materiales = new HashSet<Material>();
        }

        [Key]
        [Column(TypeName = "int")]
        public int ModeloId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo Modelo debe tener entre 5 y 50 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Column(TypeName = "ntext")]
        public string Descripcion { get; set; } = null!;

        [InverseProperty("Modelo")]
        public virtual ICollection<Material>? Materiales { get; set; }
    }
}
