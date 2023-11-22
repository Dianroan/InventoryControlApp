using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AlmacenSQLiteEntities
{
    [Table("Estudiante")]
    public class Estudiante
    {
        public Estudiante()
        {
            Pedidos = new HashSet<Pedido>();
        }

        [Key]
        [Column(TypeName = "int")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EstudianteId { get; set; }

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

        [Required(ErrorMessage = "El campo SemestreId es obligatorio.")]
        [Column(TypeName = "int")]
        [Range(1, 8, ErrorMessage = "El campo SemestreId solo se permite números del 1 al 7.")]
        public int SemestreId { get; set; }

        [Column(TypeName = "int")]
        public int? GrupoId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Adeudo { get; set; }

        [Required(ErrorMessage = "El campo Correo es obligatorio.")]
        [Column(TypeName = "nvarchar(100)")]
        [StringLength(100)]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El campo PlantelId es obligatorio.")]
        [Column(TypeName = "int")]
        [Range(1, 3, ErrorMessage = "El campo PlantelId solo debe poderse ingresar del 1 al 3.")]
        public int? PlantelId { get; set; }

        [Column(TypeName = "int")]
        public int? UsuarioId { get; set; }

        [ForeignKey("GrupoId")]
        [InverseProperty("Estudiantes")]
        public virtual Grupo Grupo { get; set; } = null!;

        [InverseProperty("Estudiante")]
        public virtual ICollection<Pedido>? Pedidos { get; set; }

        [ForeignKey("PlantelId")]
        [InverseProperty("Estudiantes")]
        public virtual Plantel Plantel { get; set; } = null!;

        [ForeignKey("SemestreId")]
        [InverseProperty("Estudiantes")]
        public virtual Semestre Semestre { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        [InverseProperty("Estudiantes")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
