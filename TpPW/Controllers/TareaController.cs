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

                //Switch de filtro
                string filtro = Request["filtrado"];
                switch (filtro)
                {
                    case "1":
                        var tareasfil = (from p in context.Tarea where p.IdUsuario == usuario && p.Completada == 0 orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                        return View(tareasfil);
                    case "2":
                        var tareascom = (from p in context.Tarea where p.IdUsuario == usuario && p.Completada == 1 orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                        return View(tareascom);
                }
                var tareas = (from p in context.Tarea where p.IdUsuario == usuario orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                return View(tareas);
            }
            else // si no existe cookies, que verifique session
            {
                if (Session["usuario"] != null)
                {
                    var usuario = Convert.ToInt32(Session["id"]);
                    string filtro = Request["filtrado"];
                    switch (filtro)
                    {
                        case "1":
                            var tareasfil = (from p in context.Tarea where p.IdUsuario == usuario && p.Completada == 0 orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                            return View(tareasfil);
                        case "2":
                            var tareascom = (from p in context.Tarea where p.IdUsuario == usuario && p.Completada == 1 orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                            return View(tareascom);
                    }
                    var tareas = (from p in context.Tarea where p.IdUsuario == usuario orderby p.Prioridad ascending, p.FechaFin descending select p).ToList();
                    return View(tareas);
                }

                else
                {
                    ViewBag.MensajeError = "El Usuario no posee Tareas";
                    return RedirectToAction("../Carpeta/MisCarpetas");
                }

            }

        }


        //Completar tarea
        public ActionResult TareaCompleta(int IdTar)
        {

            Tarea tarea = context.Tarea.FirstOrDefault(t => t.IdTarea == IdTar);
            tarea.Completada = 1;
            context.SaveChanges();

            return RedirectToAction("Home", "Home");

        }
       


        //[MyAuthorizeUsuario]
        public ActionResult NuevaTarea()
        {
            //SI existe la cookies que se cargue
            if (Request.Cookies["CookieUsuario"] != null)
            {
                ViewBag.Carpetas = CarpetasUsuario();
                return View();
            }
            else //Si no existe una cookies verifico session
            {
                if (Session["usuario"] != null)
                {
                    ViewBag.Carpetas = CarpetasUsuario();
                    return View();

                }
            }
            return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public ActionResult NuevaTarea(Tarea tarea)
        {
            //SI existe la cookies que se cargue
            if (Request.Cookies["CookieUsuario"] != null)
            {
                ViewBag.Carpetas = CarpetasUsuario();

                if (ModelState.IsValid)
                {
                    tarea.FechaCreacion = DateTime.Now;

                    if (tarea.FechaFin == null)
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
            else //Si no existe una cookies verifico session
            {
                if (Session["usuario"] != null)
                {
                    ViewBag.Carpetas = CarpetasUsuario();

                    if (ModelState.IsValid)
                    {
                        tarea.FechaCreacion = DateTime.Now;

                        if (tarea.FechaFin == null)
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
            }
            return RedirectToAction("Login", "Home");
        }

        

        public List<Tuple<int,string>> CarpetasUsuario()
        {
            if (Session["usuario"] != null)
            {
                var usuario = Convert.ToInt32( Session["id"]);

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