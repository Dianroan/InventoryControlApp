using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using AlmacenSQLiteEntities;
using AlmacenDataContext;

namespace InventoryControlPages
{
    public class PasswordModel : PageModel
    {
        private Almacen db; //Creamos el contexto de la bd, la cual es Almacen

        public PasswordModel(Almacen context)
        {
            db = context; //Le pasamos el contexto a el modelo
        }

        public void OnGet()
        {
            ViewData["Title"] = "";
        }

        //Creamos nuestras propiedades y usamos BindProperty para vincularlas con algun atributo asp
        [BindProperty]
        public string? Email { get; set; }

        [BindProperty]
        public Usuario? usuario { get; set; }

        [BindProperty]
        public string? verificationCode { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) //Si el estado de el modelo es válido
            {
                usuario = db.Usuarios.FirstOrDefault(u =>
                            u.Almacenistas.Any(a => a.Correo == Email) || //Hacemos la query que detecte el correo de el usuario, sin importar de qué tipo es
                            u.Coordinadores.Any(c => c.Correo == Email) ||
                            u.Docentes.Any(d => d.Correo == Email) ||
                            u.Estudiantes.Any(e => e.Correo == Email));
                if (usuario != null)
                {
                    string verificationCode = UI.GenerateRandomString(); //Creamos una string aleatoria y se la pasamos al código de verificación
                    TempData["VerificationCode"] = verificationCode; //Lo pasamos a un dato temporal
                    UI.SendVerificationCodeByEmail(Email, verificationCode); //Mandamos el código de verificación al email dell usuario, 
                    return RedirectToPage("/VerificationPage", new {userId = usuario.UsuarioId }); //Regresamos a la página de verifiación de el usuario que está haciendo la operación
                }
                ModelState.AddModelError(string.Empty, "No se encontró un usuario con ese correo electrónico."); //Le decimos que no pudo encontrarse el correo de el usuario
            }
            return Page();
        }

        public IActionResult OnPostIndex() //Creamos un OnPost para el íncide
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/index"); //Lo regresamos a el index, o al menú si es el caso que el modelo sea válido
            }
            return Page();
        }
    }
}
