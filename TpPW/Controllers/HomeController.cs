using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using TpPW.Models;
using System.Web.Security;
using System.Data.SqlClient;
using TpPW.Servicios;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;


namespace TpPW.Controllers
{
    public class HomeController : Controller
    { 
        public TareasEntities context = new TareasEntities();
        private CarpetaServicio CarpetaSer = new CarpetaServicio();
        

        public ActionResult Index()
      {
            Usuario CurrUser = (Usuario)Session["usuario"];

            if (Request.Cookies["CookieUsuario"] != null)
            {
                    Session["email"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieUsuarioEmail"], "CookieInfo");
                    Session["nombre"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieUsuarioNombre"], "CookieInfo");
                    Session["apellido"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieUsuarioApellido"], "CookieInfo");
                    Session["id"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieUsuarioId"], "CookieInfo");
                                
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }   
        }



        //HOME
        public ActionResult Home()
        {    //SI existe la cookies que se cargue
            if (Request.Cookies["CookieUsuario"] != null)
            {
                Session["id"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieUsuarioId"], "CookieInfo");

                var usuario = Convert.ToInt32(Session["id"]);
                var carpeta = (from c in context.Carpeta where c.IdUsuario == usuario orderby c.FechaCreacion select c);
                var tarea = (from t in context.Tarea where t.IdUsuario == usuario && t.Completada == 0 orderby t.Prioridad ascending, t.FechaFin ascending select t);

                List<object> ctobjeto = new List<object>();
                ctobjeto.Add(carpeta.ToList());
                ctobjeto.Add(tarea.ToList());

                return View(ctobjeto);
            }
            else // si no existe cookies, que verifique session
            {
                if (Session["usuario"] != null)
                {
                    var usuario = (int)Session["id"];
                    var carpeta = (from c in context.Carpeta where c.IdUsuario == usuario orderby c.FechaCreacion select c);
                    var tarea = (from t in context.Tarea where t.IdUsuario == usuario && t.Completada == 0 orderby t.Prioridad ascending, t.FechaFin ascending select t);

                    List<object> ctobjeto = new List<object>();
                    ctobjeto.Add(carpeta.ToList());
                    ctobjeto.Add(tarea.ToList());

                    return View(ctobjeto);
                }
                else
                {
                    ViewBag.MensajeError = "Usuario o contraseña invalido";
                    return RedirectToAction("../Home/Login");
                }
            }            
        }



        //Se logue al usuario
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(string email, string Contrasenia,bool Recordarme = false)
        {
            var RecordarmeValue = Request.Form["Recordarme"];

            if (email != null && Contrasenia != null) //valido que el email y la contraseña sean las correctas con las de la base
            {
                var myUsuario = context.Usuario
                                .Where(b => b.Email == email)
                                .Where(a => a.Contrasenia == Contrasenia)
                                .FirstOrDefault();

                // se recorre y se trae el primer registro
                if (myUsuario != null)
                {
                    if (myUsuario.Activo != 0) // verifico si el usuario esta activo
                    {
                        if (Recordarme)
                        {   
                            HttpCookie userCookie = new HttpCookie("CookieUsuario");
                            userCookie["CookieUsuarioId"] = ProtectCookieInfo(myUsuario.IdUsuario.ToString(), "CookieInfo");
                            userCookie["CookieUsuarioNombre"] = ProtectCookieInfo(myUsuario.Nombre, "CookieInfo");
                            userCookie["CookieUsuarioApellido"] = ProtectCookieInfo(myUsuario.Apellido, "CookieInfo");
                            userCookie["CookieUsuarioEmail"] = ProtectCookieInfo(myUsuario.Email, "CookieInfo");
                            userCookie.Expires = DateTime.Now.AddDays(1d);
                            Response.Cookies.Add(userCookie);
                            

                        }
                        else
                        {//Borro cualquier cookies que este abierta anteriormente
                            Response.Cookies.Clear();
                        }
                        
                        //capturo todos los datos en una sesion
                        Session["usuario"] = myUsuario;
                        Session["email"] = myUsuario.Email;
                        Session["nombre"] = myUsuario.Nombre;
                        Session["id"] = myUsuario.IdUsuario;
                        Session["contra"] = myUsuario.Contrasenia;

                        var usuario = Convert.ToInt32(Session["id"]);

                        var tarea = (from p in context.Tarea
                                     where p.IdUsuario == usuario 
                                     orderby p.Prioridad ascending, p.FechaFin descending
                                     select p
                                 ).ToList();

                        if (tarea != null)
                        {
                            return RedirectToAction("Home");
                        }
                        else
                        {
                            ViewBag.MensajeError = "El Usuario no posee Tareas";
                            return RedirectToAction("../Carpeta/MisCarpetas");
                        }
                        }
                        else
                        {
                           ViewBag.Messege = "El Usuario esta inactivo";
                           return RedirectToAction("../Home/Login");
                        }
                        }
                        else
                        {
                           ViewBag.Messege = "Verifique usuario y/o contraseña";
                           return RedirectToAction("../Home/Login");
                        }                  
            }
            else
            {
                ViewBag.Messege = "Verifique usuario y/o contraseña";

            }
            return RedirectToAction("Login", "Home");
        }


        



        //Cierro sesion

        public ActionResult Logout()
        {
            //Response.Cookies.Remove("CookieUsuario");
            if (Request.Cookies["CookieUsuario"] != null)
            {
                Response.Cookies["CookieUsuario"].Expires = DateTime.Now.AddDays(-1);
            }
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        





        //public void Recordarme(Usuario usuario)
        //{
        //    var RecordarmeValue = Request.Form["Recordarme"];


        //    var myUsuario = context.Usuario
        //                    .Where(b => b.Email == usuario.Email)
        //                    .Where(a => a.Contrasenia == usuario.Contrasenia)
        //                    .FirstOrDefault();

        //  if (myUsuario != null)
        //  {
                    
           
        //        //capturo todos los datos en una sesion
        //        Session["usuario"] = myUsuario;
        //        Session["email"] = myUsuario.Email;
        //        Session["nombre"] = myUsuario.Nombre;
        //        Session["id"] = myUsuario.IdUsuario;
        //        Session["contra"] = myUsuario.Contrasenia;

        //        var usu = (int)Session["id"];
        //  }
        //}
        


        //Metodos para encriptar y desencriptar la cookie
        public static string ProtectCookieInfo(string text, string purpose)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            byte[] stream = Encoding.UTF8.GetBytes(text);
            byte[] encodedValue = MachineKey.Protect(stream, purpose);
            return HttpServerUtility.UrlTokenEncode(encodedValue);
        }

        public static string UnprotectCookieInfo(string text, string purpose)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            byte[] stream = HttpServerUtility.UrlTokenDecode(text);
            byte[] decodedValue = MachineKey.Unprotect(stream, purpose);
            return Encoding.UTF8.GetString(decodedValue);
        }
    }
}