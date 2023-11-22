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
    public class GrupoModel : PageModel
    {
        private Almacen db; //Creamos el contexto de la bd, el cual es Almacen

        public GrupoModel(Almacen context)
        {
            db = context; //Se lo asignamos al Model de Grupo para que pueda usar la bd
        }

        public List<Grupo>? grupos { get; set; } //Creamos una propiedad de la tabla grupo, llamada grupos

        public void OnGet()
        {
            ViewData["Title"] = ""; //Solo obtenemos el titulo de la página
        }

        [BindProperty] //El BindProperty sirve para vincular la propiedad creada con un asp en un formulario  Se puede aplicar a una propiedad pública de un controlador o una clase PageModel para hacer que el enlace de modelos tenga esa propiedad como destino
        public Grupo? Grupo { get; set; } //Creamos una propiedad de Grupo, llamada Grupo.

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                
            }
            
            return Page(); //Se retorna a la página actual
        }
    }
}
