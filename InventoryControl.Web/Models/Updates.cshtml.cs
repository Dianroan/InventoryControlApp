using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AlmacenSQLiteEntities;
using AlmacenDataContext;

namespace InventoryControlPages
{
    public class UpdatesModel : PageModel
    {
        private Almacen db;

        public UpdatesModel(Almacen context)
        {
            db = context;
        }

        [BindProperty]
        public Categoria? categoria { get; set; } //1

        [BindProperty]
        public Material? material { get; set; } //2

        [BindProperty]
        public Marca? marca { get; set; } //3

        [BindProperty]
        public Modelo? modelo { get; set; } //4

        [BindProperty]
        public Mantenimiento? mantenimiento { get; set; } //5

        [BindProperty]
        public Estudiante? estudiante { get; set; } //6

        [BindProperty]
        public Docente? docente { get; set; } //7

        [BindProperty]
        public Almacenista? almacenista { get; set; } //8
        [BindProperty]
        public ReporteMantenimiento? reporteMantenimiento { get; set; }

        [BindProperty]
        public string tabla { get; set; }

        [BindProperty]
        public int userId { get; set; }

        [BindProperty]
        public string typeUser { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet(int id, string table, int usuario, string tipo)
        {
            tabla = table;
            userId = usuario;
            typeUser = tipo;

            switch (table)
            {
                case "categoria":
                    categoria = db.Categorias!.FirstOrDefault(c => c.CategoriaId == id);
                    break;
                case "material":
                    material = db.Materiales!.FirstOrDefault(c => c.MaterialId == id);
                    break;
                case "marca":
                    marca = db.Marcas!.FirstOrDefault(c => c.MarcaId == id);
                    break;
                case "modelo":
                    modelo = db.Modelos!.FirstOrDefault(c => c.ModeloId == id);
                    break;
                case "docente":
                    docente = db.Docentes!.FirstOrDefault(c => c.DocenteId == id);
                    break;
                case "estudiante":
                    estudiante = db.Estudiantes!.FirstOrDefault(c => c.EstudianteId == id);
                    break;
                case "almacenista":
                    almacenista = db.Almacenistas!.FirstOrDefault(c => c.AlmacenistaId == id);
                    break;
                case "mantenimiento":
                    mantenimiento = db.Mantenimientos!.FirstOrDefault(c => c.MantenimientoId == id);
                    break;
                case "reporteMantenimiento":
                    reporteMantenimiento = db.ReporteMantenimientos!.FirstOrDefault(c => c.ReporteMantenimientoId == id);
                    break;
                default:
                    break;
            }
            ViewData["Title"] = "";
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                TempData["UserType"] = 3;
                if(Request.Form["typeUser"] == "Almacenista"){
                    return RedirectToPage("/AlmacenistaMenu", new{id = int.Parse(Request.Form["userId"])});
                }
                else if(Request.Form["typeUser"] == "Coordinador"){
                    return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["userId"])});
                }
            }
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                int registroId = int.Parse(Request.Form["registroId"]);
                string tableId = Request.Form["tableId"];
                int userId = int.Parse(Request.Form["userId"]);
                string typeUser = Request.Form["typeUser"];
                string registroIdString = Request.Form["registroId"];
                
                if (string.IsNullOrEmpty(registroIdString)){
                    // Manejo de error: registroId es nulo o vacío
                    TempData["ErrorMessage"] = "El registroId no puede ser nulo o vacío.";
                    return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                }

                switch (tableId){
                    case "categoria":
                        Categoria Upcategoria = db.Categorias!.FirstOrDefault(c => c.CategoriaId == registroId);
                        Upcategoria.Nombre = categoria.Nombre;
                        Upcategoria.Descripcion = categoria.Descripcion;
                        Upcategoria.Acceso = categoria.Acceso;
                        break;
                    case "material":
                        Material Upmaterial = db.Materiales!.FirstOrDefault(c => c.MaterialId == registroId);
                        Upmaterial.ModeloId = material.ModeloId;
                        Upmaterial.Descripcion = material.Descripcion;
                        if (material.YearEntrada > DateTime.Now.Year)
                        {
                            TempData["ErrorMessage"] = "El campo Año no puede ser mayor que el año actual.";
                            return RedirectToPage("/Updates", new { id = registroId, table = tableId, usuario = userId, tipo = typeUser });
                        }
                        Upmaterial.YearEntrada = material.YearEntrada;
                        Upmaterial.MarcaId = material.MarcaId;
                        Upmaterial.CategoriaId = material.CategoriaId;
                        Upmaterial.PlantelId = material.PlantelId;
                        Upmaterial.Serie = material.Serie;
                        Upmaterial.ValorHistorico = material.ValorHistorico;
                        Upmaterial.Condicion = material.Condicion;
                        break;
                    case "marca":
                        Marca Upmarca = db.Marcas!.FirstOrDefault(c => c.MarcaId == registroId);
                        Upmarca.Nombre = marca.Nombre;
                        Upmarca.Descripcion = marca.Descripcion;
                        break;
                    case "modelo":
                        Modelo Upmodelo = db.Modelos!.FirstOrDefault(c => c.ModeloId == registroId);
                        Upmodelo.Nombre = modelo.Nombre;
                        Upmodelo.Descripcion = modelo.Descripcion;
                        break;
                    case "docente":
                        Docente Updocente = db.Docentes!.FirstOrDefault(d => d.DocenteId == registroId);
                        if(!UI.EmailValidation(docente.Correo)){
                            TempData["ErrorMessage"] = "Correo electrónico invalido";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo Nombre
                        if (string.IsNullOrWhiteSpace(docente.Nombre))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (docente.Nombre.Length < 2 || docente.Nombre.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo Nombre debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(docente.Nombre, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoPaterno
                        if (string.IsNullOrWhiteSpace(docente.ApellidoPaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (docente.ApellidoPaterno.Length < 2 || docente.ApellidoPaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(docente.ApellidoPaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoMaterno
                        if (string.IsNullOrWhiteSpace(docente.ApellidoMaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (docente.ApellidoMaterno.Length < 2 || docente.ApellidoMaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(docente.ApellidoMaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        Updocente.Nombre = docente.Nombre;
                        Updocente.ApellidoPaterno = docente.ApellidoPaterno;
                        Updocente.ApellidoMaterno = docente.ApellidoMaterno;
                        Updocente.Correo = docente.Correo;
                        Updocente.PlantelId = docente.PlantelId;
                        break;
                    case "estudiante":
                        Estudiante Upestudiante = db.Estudiantes!.FirstOrDefault(d => d.EstudianteId == registroId);
                        
                        // Validación del campo Nombre
                        if (string.IsNullOrWhiteSpace(estudiante.Nombre))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (estudiante.Nombre.Length < 2 || estudiante.Nombre.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo Nombre debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(estudiante.Nombre, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoPaterno
                        if (string.IsNullOrWhiteSpace(estudiante.ApellidoPaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (estudiante.ApellidoPaterno.Length < 2 || estudiante.ApellidoPaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(estudiante.ApellidoPaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoMaterno
                        if (string.IsNullOrWhiteSpace(estudiante.ApellidoMaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (estudiante.ApellidoMaterno.Length < 2 || estudiante.ApellidoMaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(estudiante.ApellidoMaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }
                        
                        Upestudiante.Nombre = estudiante.Nombre;
                        Upestudiante.ApellidoPaterno = estudiante.ApellidoPaterno;
                        Upestudiante.ApellidoMaterno = estudiante.ApellidoMaterno;
                        Upestudiante.PlantelId = estudiante.PlantelId;
                        Upestudiante.GrupoId = estudiante.GrupoId;
                        Upestudiante.SemestreId = estudiante.SemestreId;
                        break;
                    case "almacenista":
                        Almacenista Upalmacenista = db.Almacenistas!.FirstOrDefault(d => d.AlmacenistaId == registroId);
                        if(!UI.EmailValidation(almacenista.Correo)){
                            TempData["ErrorMessage"] = "Correo electrónico invalido";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo Nombre
                        if (string.IsNullOrWhiteSpace(almacenista.Nombre))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (almacenista.Nombre.Length < 2 || almacenista.Nombre.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo Nombre debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(almacenista.Nombre, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo Nombre solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoPaterno
                        if (string.IsNullOrWhiteSpace(almacenista.ApellidoPaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (almacenista.ApellidoPaterno.Length < 2 || almacenista.ApellidoPaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(almacenista.ApellidoPaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoPaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        // Validación del campo ApellidoMaterno
                        if (string.IsNullOrWhiteSpace(almacenista.ApellidoMaterno))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno es obligatorio.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (almacenista.ApellidoMaterno.Length < 2 || almacenista.ApellidoMaterno.Length > 50)
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno debe tener entre 2 y 50 caracteres.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        if (!Regex.IsMatch(almacenista.ApellidoMaterno, "^[a-zA-Z]+$"))
                        {
                            TempData["ErrorMessage"] = "El campo ApellidoMaterno solo debe contener letras.";
                            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                        }

                        Upalmacenista.Nombre = almacenista.Nombre;
                        Upalmacenista.ApellidoPaterno = almacenista.ApellidoPaterno;
                        Upalmacenista.ApellidoMaterno = almacenista.ApellidoMaterno;
                        Upalmacenista.Correo = almacenista.Correo;
                        Upalmacenista.PlantelId = almacenista.PlantelId;
                        break;
                    case "mantenimiento":
                        Mantenimiento Upmantenimiento = db.Mantenimientos!.FirstOrDefault(c => c.MantenimientoId == registroId);
                        Upmantenimiento.Nombre = mantenimiento.Nombre;
                        Upmantenimiento.Descripcion = Upmantenimiento.Descripcion;
                        break;
                    case "reporteMantenimiento":
                        ReporteMantenimiento UpreporteMantenimiento = db.ReporteMantenimientos!.FirstOrDefault(c => c.ReporteMantenimientoId == registroId);
                        int validateDate = UI.DateValidationWeb(reporteMantenimiento.Fecha.ToString());
                        switch (validateDate){
                            case 2:
                                TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                                return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                            case 3:
                                TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                                return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                            case 4:
                                TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                                return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                            case 5:
                                TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                                return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
                            case 1:
                                // No hay error, proceder con la lógica normal
                                break;
                        }
                        UpreporteMantenimiento.Fecha = reporteMantenimiento.Fecha;
                        break;
                    default:
                        break;
                }
                db.SaveChanges();
                return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
            }
            return Page();
        }
    }
}
