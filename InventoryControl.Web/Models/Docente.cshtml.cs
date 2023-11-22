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
    //Clase para el modelo de docente
    public class DocenteModel : PageModel
    {
        //Declaramos la propiedad para la base de datos
        private Almacen db;

        // Constructor que inicializa la base de datos
        public DocenteModel(Almacen context)
        {
            db = context;
        }

        //Declaramos la lista para objetos docentes
        public List<Docente>? docentes { get; set; }

        //Declaramos como Bind property todas las propiedades de enlace que vamos a usar para crear nuevos objetos de sus clases
        [BindProperty]
        public Docente? docente { get; set; }

        [BindProperty]
        public Pedido? pedido { get; set; } = new();

        [BindProperty]
        public DescPedido? descPedido { get; set; }
        [BindProperty]
        public Categoria? categoria { get; set; }

        //Declaramos como TempData el mensaje de error que se enviara a los usuarios
        [TempData]
        public string ErrorMessage { get; set; }

        //OnGet para obtener los datos del docente con el ID ingresado
        public void OnGet(int id)
        {
            docente = db.Docentes.FirstOrDefault(e => e.DocenteId == id);
            TempData["UserType"] = 1;
            ViewData["Title"] = "";
        }

        //Funcion OnPost para generar nuevos pedidos
        public IActionResult OnPost()
        {
            try{
                if ((pedido is not null) && (descPedido is not null) &&  !ModelState.IsValid)
                {
                    TempData["UserType"] = 1;
                    TempData["Fecha"] = pedido.Fecha;
                    TempData["HoraDevolucion"] = pedido.HoraDevolucion;

                    //Valida la fecha en la que se realizó el pedido
                    int validateDate = UI.DateValidationWeb(pedido.Fecha.ToString());
                    switch (validateDate){
                        case 2:
                            TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                            return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                        case 3:
                            TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                            return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                        case 4:
                            TempData["ErrorMessage"] = "Formato de Fecha Incorrecto.";
                            return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                        case 5:
                            TempData["ErrorMessage"] = "Rellene todos los espacios.";
                            return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                        case 1:
                            // No hay error, proceder con la lógica normal
                            break;
                    }

                    pedido.HoraEntrega = pedido.Fecha;

                    if(UI.HourValidation(pedido.HoraEntrega.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }


                    if(UI.HourValidation(pedido.HoraDevolucion.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }

                    if(pedido.HoraDevolucion <= pedido.HoraEntrega){
                        TempData["ErrorMessage"] = "La hora de devolución debe ser posterior a la hora de entrega.";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }

                    descPedido.MaterialId = UI.GetMaterialID(categoria.CategoriaId);
                    WriteLine($"{descPedido.MaterialId} |   {categoria.CategoriaId}");

                    if(descPedido.MaterialId is null || descPedido.MaterialId == 0){
                        TempData["ErrorMessage"] = "Ese material no esta disponible.";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }

                    if(descPedido.Cantidad < 1){
                        TempData["ErrorMessage"] = "No puedes introducir números negativos";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }
                    else if(descPedido.Cantidad > 10){
                        TempData["ErrorMessage"] = "No puedes poner un cantidad tan grande de materiales";
                        return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                    }

                    pedido.Estado = true;

                    CrudFuntions.AddPedido(pedido, descPedido);
                    TempData["UserType"] = 1;
                    return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
                }
                return Page();
            }
            catch(Exception ){
                TempData["ErrorMessage"] = "No puedes poner una cantidad tan grande de materiales";
                return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
            }
        }

        //Funcion OnPost para aprobar pedidos
        public IActionResult OnPostApprove()
        {
            // Obtener el valor de pedidoId del formulario
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            pedido.Estado = true;
            Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
            int? DocenteIdAux = pedido.DocenteId;
            UI.SendEmailForOrderState(estudiante,"Datos incorrectos",pedido);
            db.SaveChanges();
            TempData["UserType"] = 1;
            return RedirectToPage("/DocenteMenu", new{id = pedido.DocenteId});
        }

        //Funcion OnPost para declinar pedidos
        public IActionResult OnPostDecline()
        {
            // Obtener el valor de pedidoId del formulario            
            pedido = db.Pedidos.FirstOrDefault(p => p.PedidoId == int.Parse(Request.Form["pedidoId"]));
            pedido.Estado = false;
            descPedido = db.DescPedidos.FirstOrDefault(p => p.PedidoId == pedido.PedidoId);
            Estudiante estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
            int? DocenteIdAux = pedido.DocenteId;
            UI.SendEmailForOrderState(estudiante,"Datos incorrectos",pedido);
            db.DescPedidos.RemoveRange(descPedido);
            db.Pedidos.RemoveRange(pedido);
            db.SaveChanges();
            TempData["UserType"] = 1;
            return RedirectToPage("/DocenteMenu", new{id = DocenteIdAux});
        }
    }
}
