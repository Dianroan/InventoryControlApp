using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AlmacenSQLiteEntities
{
    [Table("Docente")]
    public class Docente
    {
        public Docente()
        {
            Pedidos = new HashSet<Pedido>();
        }

        [Key]
        [Column(TypeName = "int")]
        public int DocenteId { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo Nombre debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "El campo Nombre solo debe contener letras.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El campo ApellidoPaterno es obligatorio.")]
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo ApellidoPaterno debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El campo ApellidoPaterno solo debe contener letras.")]
        public string ApellidoPaterno { get; set; } = null!;

        [Required(ErrorMessage = "El campo ApellidoMaterno es obligatorio.")]
        [Column(TypeName = "nvarchar(50)")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El campo ApellidoMaterno debe tener entre 2 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "El campo ApellidoMaterno solo debe contener letras.")]
        public string ApellidoMaterno { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [StringLength(100)]
        public string Correo { get; set; } = null!;

        [Column(TypeName = "int")]
        public int? PlantelId { get; set; }

        [Column(TypeName = "int")]
        public int? UsuarioId { get; set; }

        [InverseProperty("Docente")]
        public virtual ICollection<Pedido>? Pedidos { get; set; }

        [ForeignKey("PlantelId")]
        [InverseProperty("Docentes")]
        public virtual Plantel Plantel { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        [InverseProperty("Docentes")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
