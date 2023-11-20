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
    public class AlmacenistaModel : PageModel
    {
        private Almacen db;

        public AlmacenistaModel(Almacen context)
        {
            db = context;
        }

        public List<Almacenista>? almacenistas { get; set; }

        [BindProperty]
        public Almacenista? almacenista { get; set; }

        [BindProperty]
        public Pedido? pedido { get; set; }

        [BindProperty]
        public DescPedido? descPedido { get; set; }

        [BindProperty]
        public Categoria? categoria { get; set; }

        [BindProperty]
        public Material? material { get; set; }

        [BindProperty]
        public Marca? marca { get; set; }

        [BindProperty]
        public Modelo? modelo { get; set; }

        public void OnGet(int id)
        {
            almacenista = db.Almacenistas.FirstOrDefault(a => a.AlmacenistaId == id);
            ViewData["Title"] = "";
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                
            }
            
            return Page();
        }

        public IActionResult OnPostApprove()
        {
            // Obtener el valor de pedidoId del formulario
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            pedido.Estado = true;
            Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
            UI.SendEmailForOrderState(estudiante,"Datos incorrectos",pedido);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostDecline()
        {
            // Obtener el valor de pedidoId del formulario            
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            pedido.Estado = false;
            descPedido = db.DescPedidos.FirstOrDefault(p => p.PedidoId == pedido.PedidoId);
            Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
            UI.SendEmailForOrderState(estudiante,"Datos incorrectos",pedido);
            db.DescPedidos.RemoveRange(descPedido);
            db.Pedidos.RemoveRange(pedido);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostNewCat()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((categoria is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddCategoria(categoria);
                TempData["UserType"] = 3;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }

        public IActionResult OnPostNewMarca()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((marca is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddMarca(marca);
                TempData["UserType"] = 3;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }

        public IActionResult OnPostNewModelo()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((modelo is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddModelo(modelo);
                TempData["UserType"] = 3;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }
        public IActionResult OnPostNewMaterial()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((material is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddMaterial(material);
                TempData["UserType"] = 3;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }
        public IActionResult OnPostDeleteOrder()
        {
            // Obtener el valor de pedidoId del formulario            
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            descPedido = db.DescPedidos.FirstOrDefault(p => p.PedidoId == pedido.PedidoId);
            db.DescPedidos.RemoveRange(descPedido);
            db.Pedidos.RemoveRange(pedido);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostDeleteCategoria()
        {
            // Obtener el valor de pedidoId del formulario            
            categoria = db.Categorias.FirstOrDefault(p => p.CategoriaId == int.Parse(Request.Form["categoriaId"]));
            foreach (var material in db.Materiales)
            {
                if(material.CategoriaId == int.Parse(Request.Form["categoriaId"])){
                    db.Materiales.RemoveRange(material);
                }
            }
            db.Categorias.RemoveRange(categoria);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostDeleteModelo()
        {
            // Obtener el valor de pedidoId del formulario            
            modelo = db.Modelos.FirstOrDefault(p => p.ModeloId == int.Parse(Request.Form["modeloId"]));
            foreach (var material in db.Materiales)
            {
                if(material.ModeloId == int.Parse(Request.Form["modeloId"])){
                    db.Materiales.RemoveRange(material);
                }
            }
            db.Modelos.RemoveRange(modelo);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostDeleteMarca()
        {
            // Obtener el valor de pedidoId del formulario            
            marca = db.Marcas.FirstOrDefault(p => p.MarcaId == int.Parse(Request.Form["marcaId"]));
            foreach (var material in db.Materiales)
            {
                if(material.MarcaId == int.Parse(Request.Form["marcaId"])){
                    db.Materiales.RemoveRange(material);
                }
            }
            db.Marcas.RemoveRange(marca);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostDeleteMaterial()
        {
            // Obtener el valor de pedidoId del formulario            
            material = db.Materiales.FirstOrDefault(p => p.MaterialId == int.Parse(Request.Form["materialId"]));
            db.Materiales.RemoveRange(material);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        public IActionResult OnPostUpdate()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            string tableId = Request.Form["tableId"];
            int userId = int.Parse(Request.Form["almacenistaId"]);
            string typeUser = "Almacenista";
            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
        }
    }
}
