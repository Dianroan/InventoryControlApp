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
using InventoryControl.UI;

namespace InventoryControlPages
{
    public class UserModel : PageModel
    {
        private Almacen db;

        public UserModel(Almacen context)
        {
            db = context;
        }

        public List<Usuario>? usuarios { get; set; }
        public List<Estudiante>? estudiantes { get; set; }

        public void OnGet()
        {
            CrudFuntions.CalcularAdeudo();
            ViewData["Title"] = "LogIn";
        }

        [BindProperty]
        public Usuario? usuario { get; set; }
        [BindProperty]
        public Estudiante? estudiante {get; set;}
        [TempData]
        public string ErrorMessageLogIn { get; set; }
        [TempData]
        public string ErrorMessageSignIn { get; set; }

        public IActionResult OnPostLogIn()
        {
            if ((usuario is not null) &&!ModelState.IsValid)
            {
                // Llama a la función de login desde tu programa de consola
                var result = UI.LogIn(usuario.Usuario1, usuario.Password);

                if(result.typeOfUser == 0){
                    TempData["ErrorMessageLogIn"] = "Usuario o contraseña incorrectos. Inténtalo nuevamente.";
                }

                if (result.usuarioEncontrado != null)
                {
                    // Almacena la información del usuario en TempData
                    TempData["UserName"] = result.usuarioEncontrado.Usuario1;
                    TempData["UserType"] = result.typeOfUser;
                    switch(result.typeOfUser){
                        case 0:
                            TempData["ErrorMessageLogIn"] = "Usuario y/o Contraseña incorrecta";
                            return Page();
                        case 1:
                            Docente? docente = db.Docentes!.FirstOrDefault(r => r.UsuarioId == result.usuarioEncontrado.UsuarioId);
                            return RedirectToPage("/DocenteMenu", new{id = docente.DocenteId});
                        case 2:
                            Estudiante? alumno = db.Estudiantes!.FirstOrDefault(r => r.UsuarioId == result.usuarioEncontrado.UsuarioId);
                            return RedirectToPage("/EstudianteMenu", new{id = alumno.EstudianteId});
                        case 3:
                            Almacenista? almacenista = db.Almacenistas!.FirstOrDefault(r => r.UsuarioId == result.usuarioEncontrado.UsuarioId);
                            return RedirectToPage("/AlmacenistaMenu", new{id = almacenista.AlmacenistaId});
                        case 4:
                            Coordinador? coordinador = db.Coordinadores!.FirstOrDefault(r => r.UsuarioId == result.usuarioEncontrado.UsuarioId);
                            return RedirectToPage("/CoordinadorMenu", new{id = coordinador.CoordinadorId});
                    }
                }
            }

            // Si las credenciales no son válidas o hay un error, permanece en la misma página
            return Page();
        }

        public IActionResult OnPostSignIn()
        {
            if ((estudiante is not null) && !ModelState.IsValid)
            {
                int validationRegister = UI.RegisterValidation(estudiante.EstudianteId);

                switch (validationRegister)
                {
                    case 1:
                        break;
                    case 10:
                        TempData["ErrorMessageSignIn"] = "El campo Registro debe tener 8 dígitos.";
                        return Page();
                    case 20:
                        TempData["ErrorMessageSignIn"] = "El año en el campo Registro no puede ser mayor al año actual.";
                        return Page();
                    case 30:
                        TempData["ErrorMessageSignIn"] = "El campo Registro debe contener con '100' o '300'.";
                        return Page();
                }
                
                int validationEmail = UI.StudentEmailValidation(estudiante.Correo, estudiante.EstudianteId);

                switch (validationEmail)
                {
                    case 1:
                        break;
                    case 10:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener 17 caracteres, ejemplo: 'a19300107@ceti.mx'";
                        return Page();
                    case 20:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener la letra a al inicio del mismo";
                        return Page();
                    case 30:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener el registro proporcionado.";
                        return Page();
                    case 40:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener la terminación 'ceti.mx'";
                        return Page();
                    case 50:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener 17 caracteres, ejemplo: 'a19300107@ceti.mx'";
                        return Page();
                    case 70:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener el registro proporcionado";
                        return Page();
                    case 80:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener la terminación 'ceti.mx'";
                        return Page();
                    case 90:
                        TempData["ErrorMessageSignIn"] = "Formato de Correo Incorrecto";
                        return Page();
                    case 100:
                        TempData["ErrorMessageSignIn"] = "El correo debe contener la terminación 'ceti.mx'";
                        return Page();
                }

                int validationPassword = UI.PasswordValidation(usuario.Password);
                
                switch (validationPassword)
                {
                    case 1:
                        // No hay error, proceder con la lógica normal
                        break;
                    case 10:
                        TempData["ErrorMessageSignIn"] = "La contraseña es muy corta. Debe tener al menos 8 caracteres.";
                        return Page();
                    case 20:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener al menos un caracter en mayusculas.";
                        return Page();
                    case 30:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener al menos un caracter numerico.";
                        return Page();
                    case 40:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener al menos un caracter especial no alfanumérico.";
                        return Page();
                    case 50:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener al menos un caracter en minúsculas.";
                        return Page();
                    case 80:
                        TempData["ErrorMessageSignIn"] = "La contraseña es muy común o fácil de adivinar.";
                        return Page();
                    case 90:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener al menos un caracter no alfanumérico.";
                        return Page();
                    case 100:
                        TempData["ErrorMessageSignIn"] = "La contraseña debe contener una combinación de mayúsculas y minúsculas.";
                        return Page();
                }

                foreach(var e in db.Estudiantes){
                    if(e.Correo == estudiante.Correo){
                        TempData["ErrorMessageSignIn"] = "El correo empleado ya tiene un usuario asignado";
                        return Page();
                    }
                }

                estudiante.Adeudo = 0;
                usuario.Temporal = false;
                usuario.Usuario1 = UI.GenerateUsername(estudiante.Nombre, estudiante.ApellidoPaterno, estudiante.ApellidoMaterno);
                TempData["UserName"] = usuario.Usuario1;
                CrudFuntions.AddStudent(estudiante, usuario);
                UI.NotificationUserName(estudiante);
                return RedirectToPage("/index", usuario.Usuario1);
            }
            TempData["ErrorMessageSignIn"] = "No se pudo hacer el registro del estudiante.";
            return Page();
        }
        public IActionResult OnPostPass()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("/PasswordPage");
            }
            return Page();
        }
    }
}
