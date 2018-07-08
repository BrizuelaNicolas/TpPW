using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TpPW.Models;
using System.Web.Security;
using System.Web.Routing;
using System.Web;

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

        {  //SI existe la cookies que se cargue
            if (Request.Cookies["CookieUsuario"] != null)
            {
                var usuario = Convert.ToInt32(Session["id"]);

                List<Tarea> tarea = (from p in context.Tarea
                                     where p.IdUsuario == usuario
                                     orderby p.Prioridad ascending, p.FechaFin descending
                                     select p
                                           ).ToList();

                return View(tarea);
            }
            else // si no existe cookies, que verifique session
            {
                if (Session["usuario"] != null)
                {
                    var usuario = Convert.ToInt32(Session["id"]);


                    List<Tarea> tarea = (from p in context.Tarea
                                         where p.IdUsuario == usuario
                                         orderby p.Prioridad ascending, p.FechaFin descending
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
                    tarea.FechaCreacion = DateTime.Now;

                    if(tarea.FechaFin == null)
                    {
                        tarea.FechaFin = DateTime.Now;
                    }                                                                               

                    var usuario = (int)Session["id"];
                   
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
                    int IdTarea = nuevocomentario.IdTarea;
                    nuevocomentario.FechaCreacion = DateTime.Now;

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





        //Metodo subir archivo
        [HttpPost]
        public ActionResult SubirArchivo(ArchivoTarea nuevoArchivo) //, HttpPostedFileBase adjunto
        {
            if (Session["usuario"] != null)
            {
                if (ModelState.IsValid)
                {
                    int IdTarea = nuevoArchivo.IdTarea;
                    //int IdTarea = IdTar;

                    //if (nuevoArchivo != null)
                    //{
                    //    string subirArchivo = ArchivoModelView.Guardar(adjunto, adjunto.FileName, $"/archivos/tareas/{IdTarea}/");
                    //    nuevoArchivo.RutaArchivo = subirArchivo;
                    //}
                    nuevoArchivo.FechaCreacion = DateTime.Now;

                    context.ArchivoTarea.Add(nuevoArchivo);
                    context.SaveChanges();

                    if (nuevoArchivo != null)
                    {
                        ViewBag.ArchivoOK = "Archivo adjuntado con exito";
                        return RedirectToAction("DescripcionTarea", "Tarea", new { @IdTar = IdTarea });
                    }
                }
                else
                {
                    ViewBag.ArchivoNo = "El archivo no pudo ser adjuntado";
                    return View("DescripcionTarea");
                }
            }
          return RedirectToAction("Login", "Home");
        }



    }
}