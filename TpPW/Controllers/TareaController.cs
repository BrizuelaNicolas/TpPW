using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TpPW.Models;
using System.Web.Security;


namespace TpPW.Controllers
{
    public class TareaController : Controller
    {
        //Conexion con sql - entities
        public TareasEntities context = new TareasEntities();


        //// GET: Tarea
        //public ActionResult Index()
        //{
        //    return View();
        //}


        //cerramos sesion
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        //Listamos las tareas
        public ActionResult MisTareas()
        {
            if (Session["usuario"] != null)
            {
                var usuario = (int)Session["id"];


               List<Tarea> tarea = (from p in context.Tarea
                                     where p.IdUsuario == usuario
                                     orderby p.FechaCreacion
                                     select p
                                          ).ToList();

                return View(tarea);
            }
            else
            {
                ViewBag.MensajeError = "El Usuario no posee Tareas";
                return RedirectToAction("../Carpeta/MisCarpetas");
            }
           
        }




        //[MyAuthorizeUsuario]
        public ActionResult NuevaTarea()
        {
            
            return View();
        }


        [HttpPost]
        public ActionResult NuevaTarea(TareaCarpeta tareacarpeta)
        {
            if (ModelState.IsValid)
            {

                if (Session["usuario"] != null)
                {

                    //falta agregar el combo solo las carpertas de ese usuario

                    string nombreTarea = tareacarpeta.tarea.Nombre;

                    string descripcionTarea = tareacarpeta.tarea.Descripcion;


                    string estimadoTarea = Convert.ToString(tareacarpeta.tarea.EstimadoHoras);
                    tareacarpeta.tarea.EstimadoHoras = Convert.ToDecimal(estimadoTarea);

                    //ver fecha, no la trae de la vista
                    DateTime FechaF = Convert.ToDateTime(tareacarpeta.tarea.FechaFin);
                    tareacarpeta.tarea.FechaFin = Convert.ToDateTime(FechaF);

                    //int Priori = tarea.Prioridad;
                    //falta la prioridad como lista dinamica.
                    tareacarpeta.tarea.Prioridad = 1;
                    //falta el id de carpeta
                    int IdCar = tareacarpeta.tarea.IdCarpeta;

                    //List<SelectListItem> priori = new SelectList((), "ItemID", "Descripcion").ToList();
                    //priori.Insert(0, (new SelectListItem { Text = " Sin Padre ", Value = 0 }));


                    var usuario = (int)Session["id"];

                    var tar = (from p in context.Tarea
                               where p.IdUsuario == usuario
                               orderby p.FechaCreacion
                               select p).ToList();

                    int IdUs = usuario;




                    tareacarpeta.tarea.IdUsuario = IdUs;
                    
                    context.Tarea.Add(tareacarpeta.tarea);
                    context.SaveChanges();
                }

                if (tareacarpeta != null)
                {
                    Session["Mensaje"] = "El paquete " + tareacarpeta.tarea.Nombre + " ha creado exitosamente";
                    return RedirectToAction("ListadoTareas");
                }

                else
                {
                    ViewBag.Mensaje = "La Tarea no pudo ser creada";
                    return View();
                }

            }

            else
            {
                ViewBag.Mensaje = "La Tarea no pudo ser creada";
                return View();
            }
        }



        public void CarpetasUsuario()
        {

            if (Session["usuario"] != null)
            {
                var usuario = (int)Session["id"];

                List<SelectListItem> car = new List<SelectListItem>();

                List<Carpeta> c = (from p in context.Carpeta
                                             where p.IdUsuario == usuario
                                             orderby p.FechaCreacion
                                               select p).ToList();
                     
                ViewData["Carpeta"] = c;

               
            }
                        
        }


        // public ActionResult Listado(int idCarpeta)
        //{
        //    List<Tarea> tareas = _tareaService.ObtenerTareasPorCarpeta(idCarpeta);
        //    TempData["idCarpeta"] = idCarpeta;
        //    return View(tareas);
        //}

        public ActionResult DescripcionTarea(int IdTar)
        {

            var listado = (from p in context.Tarea
                       join r in context.ComentarioTarea on p.IdTarea equals r.IdTarea
                       join a in context.ArchivoTarea on p.IdTarea equals a.IdTarea
                       where p.IdTarea == IdTar
                       select new TareaComentarioArchivo { tarea = p, comentario = r, archivo = a}).ToList();

            //var listaReservas = (from r in context.Tarea
            //                     join p in context.ComentarioTarea on r.IdTarea equals p.IdTarea
            //                     where r.IdUsuario == IdTar
            //                     select new TareaComentarioArchivo { comentario = r, tarea = p }).ToList();

            return View(listado);

        }
    }
}