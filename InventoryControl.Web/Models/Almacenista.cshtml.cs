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
    //creamos la clase para este modelo
    public class AlmacenistaModel : PageModel
    {
        //Declaramos la propiedad para la base de datos
        private Almacen db;

        //Instanciamos la propiedad anterior con los datos de la base de datos de almacen
        public AlmacenistaModel(Almacen context)
        {
            db = context;
        } 

        //Declaramos la lista con toda la informacion de la clase almacenistas
        public List<Almacenista>? almacenistas { get; set; }

        //Declaramos como Bind property todas las propiedades de enlace que vamos a usar para crear nuevos objetos de sus clases
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
        [BindProperty]
        public Mantenimiento? mantenimiento { get; set; }
        [BindProperty]
        public ReporteMantenimiento? reporteMantenimiento { get; set; }
        //Declaramos como TempData el mensaje de error que se enviara a los usuarios
        [TempData]
        public string ErrorMessage { get; set; }

        //OnGet para obtener los datos del almacenista con el ID ingresado
        public void OnGet(int id)
        {
            almacenista = db.Almacenistas.FirstOrDefault(a => a.AlmacenistaId == id);
            ViewData["Title"] = "";
            TempData["UserType"] = 3;
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                
            }
            
            return Page();
        }

        //Funcion OnPost para aprobar pedidos
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

        //Funcion OnPost para declinar pedidos
        public IActionResult OnPostDecline()
        {
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            //Establece el estado del pedido como rechazado
            pedido.Estado = false;
            descPedido = db.DescPedidos.FirstOrDefault(p => p.PedidoId == pedido.PedidoId);
            Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
            //Envia el Email al alumno diciendo que se declino su solicitud
            UI.SendEmailForOrderState(estudiante,"Datos incorrectos",pedido);
            db.DescPedidos.RemoveRange(descPedido);
            db.Pedidos.RemoveRange(pedido);
            db.SaveChanges();
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para crear nuevas categorías
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

        //Funcion OnPost para crear una nueva Marca
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

        //Funcion OnPost para crear un nuevo modelo
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

        //Funcion OnPost para crear un nuevo material    
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

        //Funcion OnPost para crear un nuevo mantenimiento
        public IActionResult OnPostNewMant()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((material is not null) &&  !ModelState.IsValid)
            {
                int? lastMantId = db.Mantenimientos.OrderByDescending(u => u.MantenimientoId).Select(u => u.MantenimientoId).FirstOrDefault();
                int MantID = lastMantId.HasValue ? lastMantId.Value + 1 : 1;
                mantenimiento.MantenimientoId = MantID;

                CrudFuntions.AddMant(mantenimiento);
                TempData["UserType"] = 4;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }
        
        //Funcion OnPost para crear un nuevo reporte de mantenimiento
        public IActionResult OnPostNewReportMant()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((material is not null) &&  !ModelState.IsValid)
            {
                int validateDate = UI.DateValidationWeb(reporteMantenimiento.Fecha.ToString());
                switch (validateDate){
                    case 2:
                        TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                        return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
                    case 3:
                        TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                        return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
                    case 4:
                        TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                        return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
                    case 5:
                        TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                        return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
                    case 1:
                        // No hay error, proceder con la lógica normal
                        break;
                }

                material = db.Materiales.FirstOrDefault(m => m.MaterialId == reporteMantenimiento.MaterialId);
                WriteLine(material.MaterialId);
                material.Condicion = "2";

                int? lastReportMantId = db.ReporteMantenimientos.OrderByDescending(u => u.ReporteMantenimientoId).Select(u => u.ReporteMantenimientoId).FirstOrDefault();
                int ReportMantID = lastReportMantId.HasValue ? lastReportMantId.Value + 1 : 1;
                reporteMantenimiento.ReporteMantenimientoId = ReportMantID;

                CrudFuntions.AddReporteMant(reporteMantenimiento);
                db.SaveChanges();
                TempData["UserType"] = 4;
                return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
            }
            return Page();
        }

        //Funcion OnPost para borrar una orden
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

        //Funcion OnPost para borrar una categoría
        public IActionResult OnPostDeleteCategoria()
        {
            // Obtener el valor de pedidoId del formulario            
            categoria = db.Categorias.FirstOrDefault(p => p.CategoriaId == int.Parse(Request.Form["categoriaId"]));
            if (modelo != null)
            {
                // Obtener los materiales asociados a la marca
                IQueryable<Material> materialesToDelete = db.Materiales.Where(m => m.CategoriaId == categoria.CategoriaId);

                foreach (var material in materialesToDelete)
                {
                    // Obtener Desc_Pedidos asociados al material
                    IQueryable<DescPedido> descPedidosToDelete = db.DescPedidos.Where(d => d.MaterialId == material.MaterialId);

                    foreach (var descPedido in descPedidosToDelete)
                    {
                        // Obtener Pedidos asociados al DescPedido
                        IQueryable<Pedido> pedidosToDelete = db.Pedidos.Where(p => p.PedidoId == descPedido.PedidoId);
                        db.Pedidos.RemoveRange(pedidosToDelete);
                    }

                    // Eliminar Desc_Pedidos asociados al material
                    db.DescPedidos.RemoveRange(descPedidosToDelete);

                    // Obtener ReporteMantenimientos asociados al material
                    IQueryable<ReporteMantenimiento> reportesToDelete = db.ReporteMantenimientos.Where(r => r.MaterialId == material.MaterialId);
                    
                    // Eliminar ReporteMantenimientos asociados al material
                    db.ReporteMantenimientos.RemoveRange(reportesToDelete);

                    // Eliminar el material
                    db.Materiales.Remove(material);
                }

                // Eliminar la marca
                db.Modelos.Remove(modelo);

                // Guardar los cambios en la base de datos
                db.SaveChanges();
            }
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para borrar un modelo
        public IActionResult OnPostDeleteModelo()
        {
            // Obtener el valor de pedidoId del formulario            
            modelo = db.Modelos.FirstOrDefault(p => p.ModeloId == int.Parse(Request.Form["modeloId"]));
            if (modelo != null)
            {
                // Obtener los materiales asociados a la marca
                IQueryable<Material> materialesToDelete = db.Materiales.Where(m => m.ModeloId == modelo.ModeloId);

                foreach (var material in materialesToDelete)
                {
                    // Obtener Desc_Pedidos asociados al material
                    IQueryable<DescPedido> descPedidosToDelete = db.DescPedidos.Where(d => d.MaterialId == material.MaterialId);

                    foreach (var descPedido in descPedidosToDelete)
                    {
                        // Obtener Pedidos asociados al DescPedido
                        IQueryable<Pedido> pedidosToDelete = db.Pedidos.Where(p => p.PedidoId == descPedido.PedidoId);
                        db.Pedidos.RemoveRange(pedidosToDelete);
                    }

                    // Eliminar Desc_Pedidos asociados al material
                    db.DescPedidos.RemoveRange(descPedidosToDelete);

                    // Obtener ReporteMantenimientos asociados al material
                    IQueryable<ReporteMantenimiento> reportesToDelete = db.ReporteMantenimientos.Where(r => r.MaterialId == material.MaterialId);
                    
                    // Eliminar ReporteMantenimientos asociados al material
                    db.ReporteMantenimientos.RemoveRange(reportesToDelete);

                    // Eliminar el material
                    db.Materiales.Remove(material);
                }

                // Eliminar la marca
                db.Modelos.Remove(modelo);

                // Guardar los cambios en la base de datos
                db.SaveChanges();
            }

            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para borrar una marca
        public IActionResult OnPostDeleteMarca()
        {
            // Obtener el valor de marcaId del formulario            
            Marca marca = db.Marcas.FirstOrDefault(p => p.MarcaId == int.Parse(Request.Form["marcaId"]));

            if (marca != null)
            {
                // Obtener los materiales asociados a la marca
                IQueryable<Material> materialesToDelete = db.Materiales.Where(m => m.MarcaId == marca.MarcaId);

                foreach (var material in materialesToDelete)
                {
                    // Obtener Desc_Pedidos asociados al material
                    IQueryable<DescPedido> descPedidosToDelete = db.DescPedidos.Where(d => d.MaterialId == material.MaterialId);

                    foreach (var descPedido in descPedidosToDelete)
                    {
                        // Obtener Pedidos asociados al DescPedido
                        IQueryable<Pedido> pedidosToDelete = db.Pedidos.Where(p => p.PedidoId == descPedido.PedidoId);
                        db.Pedidos.RemoveRange(pedidosToDelete);
                    }

                    // Eliminar Desc_Pedidos asociados al material
                    db.DescPedidos.RemoveRange(descPedidosToDelete);

                    // Obtener ReporteMantenimientos asociados al material
                    IQueryable<ReporteMantenimiento> reportesToDelete = db.ReporteMantenimientos.Where(r => r.MaterialId == material.MaterialId);
                    
                    // Eliminar ReporteMantenimientos asociados al material
                    db.ReporteMantenimientos.RemoveRange(reportesToDelete);

                    // Eliminar el material
                    db.Materiales.Remove(material);
                }

                // Eliminar la marca
                db.Marcas.Remove(marca);

                // Guardar los cambios en la base de datos
                db.SaveChanges();
            }
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para borrar material
        public IActionResult OnPostDeleteMaterial()
        {
            // Obtener el valor de pedidoId del formulario            
            material = db.Materiales.FirstOrDefault(p => p.MaterialId == int.Parse(Request.Form["materialId"]));
            if (material != null)
            {
                // Obtener Desc_Pedidos asociados al material
                IQueryable<DescPedido> descPedidosToDelete = db.DescPedidos.Where(d => d.MaterialId == material.MaterialId);

                foreach (var descPedido in descPedidosToDelete)
                {
                    // Obtener Pedidos asociados al DescPedido
                    IQueryable<Pedido> pedidosToDelete = db.Pedidos.Where(p => p.PedidoId == descPedido.PedidoId);
                    db.Pedidos.RemoveRange(pedidosToDelete);
                }

                // Eliminar Desc_Pedidos asociados al material
                db.DescPedidos.RemoveRange(descPedidosToDelete);

                // Obtener ReporteMantenimientos asociados al material
                IQueryable<ReporteMantenimiento> reportesToDelete = db.ReporteMantenimientos.Where(r => r.MaterialId == material.MaterialId);
                    
                // Eliminar ReporteMantenimientos asociados al material
                db.ReporteMantenimientos.RemoveRange(reportesToDelete);

                // Eliminar el material
                db.Materiales.Remove(material);

                // Guardar los cambios en la base de datos
                db.SaveChanges();
            }
            TempData["UserType"] = 3;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para borrar un mantenimiento
        public IActionResult OnPostDeleteMant()
        {
            // Obtener el valor de pedidoId del formulario            
            mantenimiento = db.Mantenimientos.FirstOrDefault(p => p.MantenimientoId == int.Parse(Request.Form["mantenimientoId"]));
            
            foreach (var reporteMantenimientos in db.ReporteMantenimientos)
            {
                if(reporteMantenimientos.MantenimientoId == int.Parse(Request.Form["mantenimientoId"])){
                    db.ReporteMantenimientos.RemoveRange(reporteMantenimientos);
                }
            }
            
            db.Mantenimientos.RemoveRange(mantenimiento);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para borrar un reporte de mantenimiento
        public IActionResult OnPostDeleteReportMant()
        {
            // Obtener el valor de pedidoId del formulario            
            reporteMantenimiento = db.ReporteMantenimientos.FirstOrDefault(p => p.ReporteMantenimientoId == int.Parse(Request.Form["reporteMantenimientoId"]));
            material = db.Materiales.FirstOrDefault(m => m.MaterialId == reporteMantenimiento.MaterialId);
            material.Condicion = "1";
            db.ReporteMantenimientos.RemoveRange(reporteMantenimiento);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["almacenistaId"])});
        }

        //Funcion OnPost para redirigir a la pagina de Update enviando los datos del mismo
        public IActionResult OnPostUpdate()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            string tableId = Request.Form["tableId"];
            int userId = int.Parse(Request.Form["almacenistaId"]);
            string typeUser = "Almacenista";
            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
        }

        //Funcion OnPost para redirigir a la pagina de entrega de material enviando los datos
        public IActionResult OnPostEntrega()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            int userId = int.Parse(Request.Form["almacenistaId"]);
            string typeUser = "Almacenista";
            return RedirectToPage("/EntregaMaterial", new{id = registroId, usuario = userId, tipo = typeUser});
        }
    }
}
