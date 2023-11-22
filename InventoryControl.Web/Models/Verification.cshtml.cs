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
    public class VerificationModel : PageModel
    {
        private Almacen db;

        // Constructor que recibe un contexto de base de datos Almacen
        public VerificationModel(Almacen context)
        {
            db = context;
        }

        // Propiedad para enlazar con el código ingresado en la página
        [BindProperty]
        public string? Code { get; set; }

        // Método llamado cuando se realiza una solicitud GET a la página
        [HttpGet]
        public void OnGet(int userId)
        {
            // Almacena el userId en TempData para su uso posterior
            TempData["userId"] = userId;
            // Establece el título de la vista
            ViewData["Title"] = "";
        } 

        // Método llamado cuando se realiza una solicitud POST a la página
        [HttpPost]
        public IActionResult OnPost()
        {
            // Obtiene el código almacenado en TempData
            string savedCode = TempData["VerificationCode"].ToString();
            try
            {
                // Compara el código ingresado con el código almacenado
                if (Code == savedCode)
                {
                    if (string.Equals(Request.Form["Code"], savedCode))
                    {
                        // Redirecciona a la página NewPassword si los códigos coinciden
                        return RedirectToPage("/NewPassword", new{id = int.Parse(TempData["userId"].ToString())});
                    }
                    else{
                        TempData["VerificationCode"] = savedCode;
                    }
                }
                else{
                    TempData["VerificationCode"] = savedCode;
                }
                // Muestra un error si los códigos no coinciden
                ModelState.AddModelError(string.Empty, "El código es incorrecto.");
                return Page();
            }
            catch (Exception ex)
            {
                throw; // O manejar el error según sea necesario.
            }
        }

        // Método llamado cuando se realiza una solicitud POST a la página con el nombre "Index"
        public IActionResult OnPostIndex()
        {
            // Redirecciona a la página de inicio si el modelo es válido
            if (ModelState.IsValid)
            {
                return RedirectToPage("/index");
            }
            return Page();
        }
    }
}
