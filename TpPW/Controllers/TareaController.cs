using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TpPW.Models;
using System.Web.Security;
using System.Web.Routing;

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



        ////Listamos las tareas
        //public ActionResult MisTareas()
        //{
        //    if (Session["usuario"] != null)
        //    {
        //        var usuario = (int)Session["id"];

        //        MisTareasViewModel model = new MisTareasViewModel();

        //        var tareas = (from p in context.Tarea
        //                      join q in context.Carpeta on p.IdUsuario equals q.IdUsuario
        //                      where p.IdUsuario == usuario
        //                      orderby p.FechaCreacion
        //                      select new { p, q.Nombre }
        //                                   ).ToList();

        //        foreach (var item in tareas)
        //        {
        //            model.Tareas.Add(new Tuple<Tarea, string>(item.p, item.Nombre));
        //        }

        //        return View(model);
        //    }
        //    else
        //    {
        //        ViewBag.MensajeError = "El Usuario no posee Tareas";
        //        return RedirectToAction("../Carpeta/MisCarpetas");
        //    }

        //}




        //Listamos las tareas
        public ActionResult MisTareas()
        {
            if (Session["usuario"] != null)
            {
                var usuario = (int)Session["id"];

//List<Tarea>

                 var tarea = (from p in context.Tarea
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
            if (Session["usuario"] != null)
            {
                ViewBag.Carpetas = CarpetasUsuario();
                return View();
            }
            return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public ActionResult NuevaTarea(Tarea tarea)
        {
            if (Session["usuario"] != null)
            {
                ViewBag.Carpetas = CarpetasUsuario();

                if (ModelState.IsValid)
                {

                    //falta agregar el combo solo las carpertas de ese usuario
                    tarea.FechaCreacion = DateTime.Now;
                    if(tarea.FechaFin == null)
                    {
                        tarea.FechaFin = DateTime.Now;
                    }                                                                               

                    var usuario = (int)Session["id"];

                    //var tar = (from p in context.Tarea
                    //           where p.IdUsuario == usuario
                    //           orderby p.FechaCreacion
                    //           select p).ToList();                    

                    tarea.IdUsuario = usuario;

                    context.Tarea.Add(tarea);
                    context.SaveChanges();

                    if (tarea != null)
                    {
                        Session["Mensaje"] = "La tarea " + tarea.Nombre + " se ha creado exitosamente";
                        return RedirectToAction("MisTareas");
                    }
                }
                else
                {
                    ViewBag.Mensaje = "La Tarea no pudo ser creada";
                    return View("NuevaTarea");
                }
            }
            return RedirectToAction("Login", "Home");
        }


        public List<Tuple<int,string>> CarpetasUsuario()
        {
            if (Session["usuario"] != null)
            {
                var usuario = (int)Session["id"];

                var c = (from p in context.Carpeta
                                             where p.IdUsuario == usuario
                                             orderby p.FechaCreacion
                                               select p);

                List<Tuple<int, string>> newList = new List<Tuple<int, string>>();
                foreach (var carpeta in c)
                {
                    newList.Add(new Tuple<int, string>(carpeta.IdCarpeta, carpeta.Nombre));
                }
                return newList;
            }
            return null;
        }

    

        public ActionResult DescripcionTarea(int IdTar)
        {
            DetalleTarea model = new DetalleTarea();

            model.Tarea = (from p in context.Tarea
                         where p.IdTarea == IdTar select p).Single();

            model.ListaComentarioTarea = (from p in context.ComentarioTarea
                                 where p.IdTarea == IdTar
                                 select p).ToList();

            model.ListaArchivoTarea = (from p in context.ArchivoTarea
                                 where p.IdTarea == IdTar
                                 select p).ToList();
            
            return View(model);

        }

     

        [HttpPost]
        public ActionResult CrearComentario(ComentarioTarea nuevocomentario)
        {
            if (Session["usuario"] != null)
          {

                if (ModelState.IsValid)
                {

                    string NombreCarp = nuevocomentario.Texto;

                    int IdTarea = nuevocomentario.IdTarea;

                    nuevocomentario.FechaCreacion = DateTime.Now;


                    //var usuario = (int)Session["id"];

                    //var tarea = (from p in context.Tarea
                    //           where p.IdUsuario == usuario
                    //           orderby p.FechaCreacion
                    //           select p).ToList();

                    context.ComentarioTarea.Add(nuevocomentario);
                    context.SaveChanges();

                    if (nuevocomentario != null)
                    {
                        ViewBag.Mensaje = "Comentario Creado con exito";
                        return RedirectToAction("DescripcionTarea", "Tarea", new {@IdTar = IdTarea});
                    }
                }
                
                else
                {
                    ViewBag.Mensaje = "El Comentario no pudo ser creado";
                    return View("DescripcionTarea");
                }

              }


            return RedirectToAction("Login", "Home");
        }



    }
}