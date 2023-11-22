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
    public class EstudianteModel : PageModel
    {
        private Almacen db;

        public EstudianteModel(Almacen context)
        {
            db = context;
        }

        public List<Estudiante>? estudiantes { get; set; }

        [BindProperty]
        public Estudiante? estudiante { get; set; }
        [BindProperty]
        public Pedido? pedido { get; set; } = new();

        [BindProperty]
        public DescPedido? descPedido { get; set; }
        [BindProperty]
        public Categoria? categoria { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public void OnGet(int id)
        {
            estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == id);
            TempData["UserType"] = 2;
            ViewData["Title"] = "";
        }

        public IActionResult OnPostNewOrder()
        {
            try{

                if ((pedido is not null) && (descPedido is not null) &&  !ModelState.IsValid)
                {
                    TempData["UserType"] = 2;
                    TempData["Fecha"] = pedido.Fecha;
                    TempData["HoraDevolucion"] = pedido.HoraDevolucion;

                    int validateDate = UI.DateValidationWeb(pedido.Fecha.ToString());
                    switch (validateDate){
                        case 2:
                            TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                            return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                        case 3:
                            TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                            return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                        case 4:
                            TempData["ErrorMessage"] = "Formato de Fecha Incorrecto.";
                            return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                        case 5:
                            TempData["ErrorMessage"] = "Rellene todos los espacios.";
                            return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                        case 1:
                            // No hay error, proceder con la lógica normal
                            break;
                    }

                    pedido.HoraEntrega = pedido.Fecha;

                    if(UI.HourValidation(pedido.HoraEntrega.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }


                    if(UI.HourValidation(pedido.HoraDevolucion.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }

                    if(pedido.HoraDevolucion <= pedido.HoraEntrega){
                        TempData["ErrorMessage"] = "La hora de devolución debe ser posterior a la hora de entrega.";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }

                    descPedido.MaterialId = UI.GetMaterialID(categoria.CategoriaId);
                    WriteLine($"{descPedido.MaterialId} |   {categoria.CategoriaId}");

                    if(descPedido.MaterialId is null || descPedido.MaterialId == 0){
                        TempData["ErrorMessage"] = "Ese material no esta disponible.";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }

                    if(descPedido.Cantidad < 1){
                        TempData["ErrorMessage"] = "No puedes introducir números negativos";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }
                    else if(descPedido.Cantidad > 10){
                        TempData["ErrorMessage"] = "No puedes poner un cantidad tan grande de materiales";
                        return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                    }

                    WriteLine($"{pedido.EstudianteId}");
                    CrudFuntions.AddPedido(pedido, descPedido);
                    
                    // Para enviarle el correo al docente
                    estudiante = db.Estudiantes.FirstOrDefault(e => e.EstudianteId == pedido.EstudianteId);
                    WriteLine($"{estudiante.EstudianteId}");
                    //UI.SendNotTeacher(estudiante,descPedido,pedido);
                    return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
                }
                return Page();
            }
            catch(Exception ){
                TempData["ErrorMessage"] = "No puedes poner una cantidad tan grande de materiales";
                return RedirectToPage("/EstudianteMenu", new{id = pedido.EstudianteId});
            }
        }
    }
}
