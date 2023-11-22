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
    public class CoordinadorModel : PageModel
    {
        private Almacen db;

        public CoordinadorModel(Almacen context)
        {
            db = context;
        }

        public List<Coordinador>? coordinadores { get; set; }

        [BindProperty]
        public Coordinador? coordinador { get; set; }

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
        public Usuario? usuario { get; set; }
        [BindProperty]
        public Estudiante? estudiante {get; set;}
        [BindProperty]
        public Docente? docente {get; set;}
        [BindProperty]
        public Almacenista? almacenista { get; set; }
        [BindProperty]
        public Grupo? grupo { get; set; }
        [BindProperty]
        public Mantenimiento? mantenimiento { get; set; }
        [BindProperty]
        public ReporteMantenimiento? reporteMantenimiento { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet(int id)
        {
            coordinador = db.Coordinadores.FirstOrDefault(a => a.CoordinadorId == id);
            ViewData["Title"] = "";
            TempData["UserType"] = 4;
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
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
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
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

        public IActionResult OnPostNewOrder()
        {
            try{
                
                if ((pedido is not null) && (descPedido is not null) &&  !ModelState.IsValid)
                {
                    int validateDate = UI.DateValidationWeb(pedido.Fecha.ToString());
                    switch (validateDate){
                        case 2:
                            TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                        case 3:
                            TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                        case 4:
                            TempData["ErrorMessage"] = "Formato de Fecha Incorrecto.";
                            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                        case 5:
                            TempData["ErrorMessage"] = "Rellene todos los espacios.";
                            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                        case 1:
                            // No hay error, proceder con la lógica normal
                            break;
                    }

                    pedido.HoraEntrega = pedido.Fecha;

                    if(UI.HourValidation(pedido.HoraEntrega.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }


                    if(UI.HourValidation(pedido.HoraDevolucion.ToString()) == false){
                        TempData["ErrorMessage"] = "Horario no válido. Inténtalo de nuevo.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }

                    if(pedido.HoraDevolucion <= pedido.HoraEntrega){
                        TempData["ErrorMessage"] = "La hora de devolución debe ser posterior a la hora de entrega.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }

                    descPedido.MaterialId = UI.GetMaterialID(categoria.CategoriaId);
                    WriteLine($"{descPedido.MaterialId} |   {categoria.CategoriaId}");

                    if(descPedido.MaterialId is null || descPedido.MaterialId == 0){
                        TempData["ErrorMessage"] = "Ese material no esta disponible.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }

                    if(descPedido.Cantidad < 1){
                        TempData["ErrorMessage"] = "No puedes introducir números negativos";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }
                    else if(descPedido.Cantidad > 10){
                        TempData["ErrorMessage"] = "No puedes poner un cantidad tan grande de materiales";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    }

                    WriteLine($"{pedido.CoordinadorId}");
                    CrudFuntions.AddPedido(pedido, descPedido);

                    pedido.Estado = true;
                    
                    TempData["UserType"] = 4;
                    return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                }
                return Page();
            }
            catch(Exception ){
                TempData["ErrorMessage"] = "No puedes poner una cantidad tan grande de materiales";
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
        }

        public IActionResult OnPostNewCat()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((categoria is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddCategoria(categoria);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }

        public IActionResult OnPostNewMarca()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((marca is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddMarca(marca);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }

        public IActionResult OnPostNewModelo()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((modelo is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddModelo(modelo);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }
        public IActionResult OnPostNewMaterial()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((material is not null) &&  !ModelState.IsValid)
            {
                CrudFuntions.AddMaterial(material);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }
        
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
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }
        
        public IActionResult OnPostNewReportMant()
        {
            // Obtener el valor de pedidoId del formulario  
            if ((material is not null) &&  !ModelState.IsValid)
            {
                int validateDate = UI.DateValidationWeb(reporteMantenimiento.Fecha.ToString());
                switch (validateDate){
                    case 2:
                        TempData["ErrorMessage"] = "No se permiten selecciones en sábados ni domingos.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 3:
                        TempData["ErrorMessage"] = "La fecha debe ser un día posterior al día actual y no mayor a una semana.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 4:
                        TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 5:
                        TempData["ErrorMessage"] = "Formato de fecha incorrecto. Intenta de nuevo.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
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
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }

       public IActionResult OnPostNewGrupo(Grupo grupo)
        {
            // Verifica si el grupo ya existe
            bool grupoExistente = db.Grupos.Any(g => g.Nombre == grupo.Nombre);

            if (grupoExistente)
            {
                // Establece el mensaje de error en el modelo
                TempData["ErrorMessage"] = "El grupo ya existe. Introduce un nombre de grupo único.";
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }

            // Valida el formato del nombre (una letra seguida de un número)
            if (!Regex.IsMatch(grupo.Nombre, "^[A-Za-z][0-9]$"))
            {
                // Establece el mensaje de error en el modelo
                TempData["ErrorMessage"] = "El formato del nombre no es válido. Debe ser una letra seguida de un número.";
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }

            int? lastGroupId = db.Grupos.OrderByDescending(u => u.GrupoId).Select(u => u.GrupoId).FirstOrDefault();
            int GroupId = lastGroupId.HasValue ? lastGroupId.Value + 1 : 1;
            grupo.GrupoId = GroupId;

            // Si el grupo no existe, procede a guardarlo en la base de datos
            db.Grupos.Add(grupo);
            db.SaveChanges();

            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

        public IActionResult OnPostNewEstudiante()
        {
            if ((estudiante is not null) && !ModelState.IsValid)
            {
                int validationRegister = UI.RegisterValidation(estudiante.EstudianteId);

                switch (validationRegister)
                {
                    case 10:
                        TempData["ErrorMessage"] = "El campo Registro debe tener 8 dígitos.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 20:
                        TempData["ErrorMessage"] = "El año en el campo Registro no puede ser mayor al año actual.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 30:
                        TempData["ErrorMessage"] = "El campo Registro debe comenzar con '100' o '300'.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 01:
                        // No hay error, proceder con la lógica normal
                        break;
                }
                
                int validationEmail = UI.StudentEmailValidation(estudiante.Correo, estudiante.EstudianteId);

                switch (validationEmail)
                {
                    case 10:
                        TempData["ErrorMessage"] = "El correo debe contener 17 caracteres, ejemplo: 'a19300107@ceti.mx'";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 20:
                        TempData["ErrorMessage"] = "El correo debe contener la letra a al inicio del mismo";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 30:
                        TempData["ErrorMessage"] = "El correo debe contener el registro proporcionado.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 40:
                        TempData["ErrorMessage"] = "El correo debe contener la terminación 'ceti.mx'";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 50:
                        TempData["ErrorMessage"] = "El correo debe contener 17 caracteres, ejemplo: 'a19300107@ceti.mx'";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 70:
                        TempData["ErrorMessage"] = "El correo debe contener el registro proporcionado";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 80:
                        TempData["ErrorMessage"] = "El correo debe contener la terminación 'ceti.mx'";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 90:
                        TempData["ErrorMessage"] = "Formato de Correo Incorrecto";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 100:
                        TempData["ErrorMessage"] = "El correo debe contener la terminación 'ceti.mx'";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 01:
                        // No hay error, proceder con la lógica normal
                        break;
                }

                int validationPassword = UI.PasswordValidation(usuario.Password);
                
                switch (validationPassword)
                {
                    case 10:
                        TempData["ErrorMessage"] = "La contraseña es muy corta. Debe tener al menos 8 caracteres.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 20:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en mayusculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 30:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter numerico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 40:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter especial no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 50:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 80:
                        TempData["ErrorMessage"] = "La contraseña es muy común o fácil de adivinar.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 90:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 100:
                        TempData["ErrorMessage"] = "La contraseña debe contener una combinación de mayúsculas y minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 01:
                        // No hay error, proceder con la lógica normal
                        break;
                }

                estudiante.Adeudo = 0;
                usuario.Temporal = false;
                usuario.Usuario1 = UI.GenerateUsername(estudiante.Nombre, estudiante.ApellidoPaterno, estudiante.ApellidoMaterno);
                TempData["UserName"] = usuario.Usuario1;
                CrudFuntions.AddStudent(estudiante, usuario);
                //UI.NotificationUserName(estudiante);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }
        public IActionResult OnPostNewDocente()
        {
            if ((estudiante is not null) && !ModelState.IsValid)
            {
                if(!UI.EmailValidation(docente.Correo)){
                    TempData["ErrorMessage"] = "Correo electrónico invalido";
                    return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                }

                int validationPassword = UI.PasswordValidation(usuario.Password);
                
                switch (validationPassword)
                {
                    case 10:
                        TempData["ErrorMessage"] = "La contraseña es muy corta. Debe tener al menos 8 caracteres.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 20:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en mayusculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 30:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter numerico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 40:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter especial no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 50:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 80:
                        TempData["ErrorMessage"] = "La contraseña es muy común o fácil de adivinar.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 90:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 100:
                        TempData["ErrorMessage"] = "La contraseña debe contener una combinación de mayúsculas y minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 01:
                        // No hay error, proceder con la lógica normal
                        break;
                }

                usuario.Temporal = false;
                usuario.Usuario1 = UI.GenerateUsername(docente.Nombre, docente.ApellidoPaterno, docente.ApellidoMaterno);
                TempData["UserName"] = usuario.Usuario1;
                CrudFuntions.AddTeacher(docente, usuario);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
            }
            return Page();
        }
        
        public IActionResult OnPostNewAlmacenista()
        {
            if ((estudiante is not null) && !ModelState.IsValid)
            {
                if(!UI.EmailValidation(almacenista.Correo)){
                    TempData["ErrorMessage"] = "Correo electrónico invalido";
                    return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                }

                int validationPassword = UI.PasswordValidation(usuario.Password);
                
                switch (validationPassword)
                {
                    case 10:
                        TempData["ErrorMessage"] = "La contraseña es muy corta. Debe tener al menos 8 caracteres.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 20:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en mayusculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 30:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter numerico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 40:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter especial no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 50:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter en minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 80:
                        TempData["ErrorMessage"] = "La contraseña es muy común o fácil de adivinar.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 90:
                        TempData["ErrorMessage"] = "La contraseña debe contener al menos un caracter no alfanumérico.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 100:
                        TempData["ErrorMessage"] = "La contraseña debe contener una combinación de mayúsculas y minúsculas.";
                        return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
                    case 01:
                        // No hay error, proceder con la lógica normal
                        break;
                }

                usuario.Temporal = false;
                usuario.Usuario1 = UI.GenerateUsername(almacenista.Nombre, almacenista.ApellidoPaterno, almacenista.ApellidoMaterno);
                TempData["UserName"] = usuario.Usuario1;
                CrudFuntions.AddWarehouseManager(almacenista, usuario);
                TempData["UserType"] = 4;
                return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
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
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

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
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

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
            
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

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

            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new { id = int.Parse(Request.Form["coordinadorId"]) });
        }

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
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }
        
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
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

        public IActionResult OnPostDeleteReportMant()
        {
            // Obtener el valor de pedidoId del formulario            
            reporteMantenimiento = db.ReporteMantenimientos.FirstOrDefault(p => p.ReporteMantenimientoId == int.Parse(Request.Form["reporteMantenimientoId"]));
            material = db.Materiales.FirstOrDefault(m => m.MaterialId == reporteMantenimiento.MaterialId);
            material.Condicion = "1";
            db.ReporteMantenimientos.RemoveRange(reporteMantenimiento);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }
        
        public IActionResult OnPostDeleteGrupo()
        {
            // Obtener el valor de pedidoId del formulario            
            grupo = db.Grupos.FirstOrDefault(p => p.GrupoId == int.Parse(Request.Form["grupoId"]));
            db.Grupos.RemoveRange(grupo);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }
        
        public IActionResult OnPostDeleteEstudiante()
        {
            // Obtener el valor de estudianteId del formulario            
            estudiante = db.Estudiantes.FirstOrDefault(p => p.EstudianteId == int.Parse(Request.Form["estudianteId"]));
            // Obtener los pedidos y desc_pedidos asociados al estudiante
            IQueryable<Pedido> pedidos = db.Pedidos!.Where(p => p.EstudianteId == estudiante.EstudianteId);
            IQueryable<DescPedido> descPedidos = db.DescPedidos!.Where(dp => pedidos.Any(p => p.PedidoId == dp.PedidoId));

            // Eliminar los desc_pedidos asociados
            db.DescPedidos.RemoveRange(descPedidos);

            // Eliminar los pedidos asociados
            db.Pedidos.RemoveRange(pedidos);

            // Eliminar el usuario asociado al estudiante
            db.Usuarios.RemoveRange(estudiante.Usuario);

            // Eliminar al estudiante
            db.Estudiantes.RemoveRange(estudiante);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }
        
        public IActionResult OnPostDeleteDocente()
        {
            // Obtener el valor de estudianteId del formulario            
            docente = db.Docentes.FirstOrDefault(p => p.DocenteId == int.Parse(Request.Form["docenteId"]));
            // Obtener los pedidos y desc_pedidos asociados al estudiante
            IQueryable<Pedido> pedidos = db.Pedidos!.Where(p => p.DocenteId == docente.DocenteId);
            IQueryable<DescPedido> descPedidos = db.DescPedidos!.Where(dp => pedidos.Any(p => p.PedidoId == dp.PedidoId));

            // Eliminar los desc_pedidos asociados
            db.DescPedidos.RemoveRange(descPedidos);

            // Eliminar los pedidos asociados
            db.Pedidos.RemoveRange(pedidos);

            // Eliminar el usuario asociado al estudiante
            db.Usuarios.RemoveRange(docente.Usuario);

            // Eliminar al estudiante
            db.Docentes.RemoveRange(docente);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }
        
        public IActionResult OnPostDeleteAlmacenista()
        {
            // Obtener el valor de estudianteId del formulario            
            almacenista = db.Almacenistas.FirstOrDefault(p => p.AlmacenistaId == int.Parse(Request.Form["almacenistaId"]));

            // Eliminar el usuario asociado al estudiante
            db.Usuarios.RemoveRange(almacenista.Usuario);

            // Eliminar al estudiante
            db.Almacenistas.RemoveRange(almacenista);
            db.SaveChanges();
            TempData["UserType"] = 4;
            return RedirectToPage("/CoordinadorMenu", new{id = int.Parse(Request.Form["coordinadorId"])});
        }

        public IActionResult OnPostUpdate()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            string tableId = Request.Form["tableId"];
            int userId = int.Parse(Request.Form["coordinadorId"]);
            string typeUser = "Coordinador";
            return RedirectToPage("/Updates", new{id = registroId, table = tableId, usuario = userId, tipo = typeUser});
        }
        public IActionResult OnPostEntrega()
        {
            // Obtener el valor de pedidoId del formulario  
            int registroId = int.Parse(Request.Form["registroId"]);
            int userId = int.Parse(Request.Form["coordinadorId"]);
            string typeUser = "Coordinador";
            return RedirectToPage("/EntregaMaterial", new{id = registroId, usuario = userId, tipo = typeUser});
        }
    }
}
