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

        // GET: Home

        public TareasEntities context = new TareasEntities();

        private CarpetaServicio CarpetaSer = new CarpetaServicio();


        public ActionResult Index()
      {
            Usuario CurrUser = (Usuario)Session["usuario"];

            if (Request.Cookies["CookieUsuario"] != null)
            {
                if (CurrUser != null)
                {
                    Session["email"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieEmail"], "CookieInfo");
                    Session["contra"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieContra"], "CookieInfo");
                    Session["Id"] = UnprotectCookieInfo(Request.Cookies["CookieUsuario"]["CookieId"], "CookieInfo");

                    return RedirectToAction("Index");
                }
                ViewBag.Messege = "Usuario y/o contraseña incorrectos";
                return View("Index");
            }
            else
            {
                return RedirectToAction("Login", "Home");

            }
   
        }



        //HOME
        public ActionResult Home()
        {
            if (Session["usuario"] != null)
            {
                var usuario = (int)Session["id"];

                var carpeta = (from c in context.Carpeta where c.IdUsuario == usuario orderby c.FechaCreacion select c);

                var tarea = (from t in context.Tarea where t.IdUsuario == usuario orderby t.FechaCreacion select t);

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



        //Se logue al usuario
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string Contrasenia)
        {

            if (email != null && Contrasenia != null) //valido que el email y la contraseña sean las correctas con las de la base
            {
                string RecordarmeValue = Request["Recordarme"];
                System.Diagnostics.Debug.WriteLine("Login - Remember Me: " + RecordarmeValue);

                var myUsuario = context.Usuario
                                .Where(b => b.Email == email)
                                .Where(a => a.Contrasenia == Contrasenia)
                                .FirstOrDefault();

                // se recorre y se trae el primer registro
                              

                if (myUsuario != null)
                {
                    if (myUsuario.Activo != 0) // verifico si el usuario esta activo
                    {
                        if (RecordarmeValue.Equals("true"))
                        {
                            HttpCookie userCookie = new HttpCookie("CookieUsuario");
                            userCookie["CookieId"] = ProtectCookieInfo(myUsuario.IdUsuario.ToString(), "CookieInfo");
                            userCookie["CookieEmail"] = ProtectCookieInfo(myUsuario.Email, "CookieInfo");
                            userCookie["CookieContra"] = ProtectCookieInfo(myUsuario.Contrasenia, "CookieInfo");
                            userCookie.Expires = DateTime.Now.AddDays(1d);
                            Response.Cookies.Add(userCookie);
                            System.Diagnostics.Debug.WriteLine("Login - Cookie Usuario Id: " + userCookie["CookieUsuarioId"]);
                        }
                        //capturo todos los datos en una sesion
                        Session["usuario"] = myUsuario;
                        Session["email"] = myUsuario.Email;
                        Session["nombre"] = myUsuario.Nombre;
                        Session["id"] = myUsuario.IdUsuario;
                        Session["contra"] = myUsuario.Contrasenia;


                        var usuario = (int)Session["id"];

                        var tarea = (from p in context.Tarea
                                     where p.IdUsuario == usuario
                                     orderby p.FechaCreacion
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
                             }


                      }
                          else
                           {
                           ViewBag.Messege = "Verifique usuario y/o contraseña";
                        
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
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


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