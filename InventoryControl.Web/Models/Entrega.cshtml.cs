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
    //Clase para el modelo de entrega
    public class EntregaModel : PageModel
    {
        //Declaramos la propiedad para la base de datos
        private Almacen db;

        // Constructor que inicializa la base de datos
        public EntregaModel(Almacen context)
        {
            db = context;
        }

        //Declaramos como Bind property todas las propiedades de enlace que vamos a usar para crear nuevos objetos de sus clases
        [BindProperty]
        public int userId { get; set; }

        [BindProperty]
        public string typeUser { get; set; }

        [BindProperty]
        public Material? material { get; set; }

        //OnGet para obtener el ID del equipo, el ID del usuario que va a realizar la entrega y el tipo de usuario
        public void OnGet(int id, int usuario, string tipo){
            userId = usuario;
            typeUser = tipo;
            material = db.Materiales!.FirstOrDefault(c => c.MaterialId == id);
        }

        //Funcion OnPost para regresar al menu ya sea de almacenista o de coordinador
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                //Regresa al menu de Almacenista
                if(Request.Form["typeUser"] == "Almacenista"){
                    TempData["UserType"] = 3;
                    
                    return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["userId"])});
                }
                //Regresa al menu de Coordinador
                else if(Request.Form["typeUser"] == "Coordinador"){
                    TempData["UserType"] = 4;
                    return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["userId"])});
                }
            }
            return Page();
        }

        //Funcion OnPost para generar la nueva entrega
        public IActionResult OnPostNewEntrega()
        {
            if (!ModelState.IsValid)
            {
                int registroId = int.Parse(Request.Form["registroId"]);
                int userId = int.Parse(Request.Form["userId"]);
                string typeUser = Request.Form["typeUser"];
                Material Upmaterial = db.Materiales!.FirstOrDefault(c => c.MaterialId == registroId);
                Upmaterial.Condicion = material.Condicion;
                db.SaveChanges();
                if(Request.Form["typeUser"] == "Almacenista"){
                    TempData["UserType"] = 3;
                    return RedirectToPage("/EntregaMaterial", new{id = registroId, usuario = userId, tipo = typeUser});
                }
                else if(Request.Form["typeUser"] == "Coordinador"){
                    TempData["UserType"] = 4;
                    return RedirectToPage("/EntregaMaterial", new{id = registroId, usuario = userId, tipo = typeUser});
                }
            }
            return Page();
        }
        //Funcion OnPost para entrega de materiales prestados
        public IActionResult OnPostEntrega()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            int userId = int.Parse(Request.Form["userId"]);
            string typeUser = Request.Form["typeUser"];
            return RedirectToPage("/EntregaMaterial", new{id = registroId, usuario = userId, tipo = typeUser});
        }
    }
}
