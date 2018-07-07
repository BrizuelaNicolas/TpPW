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



        //Se logue al usuario
        [HttpGet]
        public ActionResult Login()
        {
            //context.Configuration.ValidateOnSaveEnabled
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string Contrasenia)
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
                        
                        //capturo todos los datos en una sesion
                        Session["usuario"] = myUsuario;
                        Session["email"] = myUsuario.Email;
                        Session["nombre"] = myUsuario.Nombre;
                        Session["id"] = myUsuario.IdUsuario;
                        Session["contra"] = myUsuario.Contrasenia;


                        var usuario = (int)Session["id"];

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


        public void Recordarme(string email, string Contrasenia)
        {
            var RecordarmeValue = Request.Form["Recordarme"];

            var myUsuario = context.Usuario
                            .Where(b => b.Email == email)
                            .Where(a => a.Contrasenia == Contrasenia)
                            .FirstOrDefault();

          if (myUsuario != null)
          {
                    if (RecordarmeValue.Equals("true"))
                    {

                        //Borro cualquier cookies guardada anteriormente
                            Response.Cookies.Clear();
                        // establecer la nueva fecha de caducidad - a treinta días a partir de ahora 
                            DateTime expiryDate = DateTime.Now.AddDays(30);
                            //userCookie.Expires.AddMinutes(1);

                        // crear un nuevo ticket de autenticación de formularios
                            FormsAuthenticationTicket Usu = new FormsAuthenticationTicket(2, myUsuario.Contrasenia, DateTime.Now, expiryDate, true, String.Empty);
                        // cifro usuario
                            string encryptedUsu = FormsAuthentication.Encrypt(Usu);
                        // creo una nueva cookie de autenticación y establece su fecha de caducidad 
                            HttpCookie userCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedUsu);


                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["Email"] = myUsuario.Email;
                    userInfo["Contra"] = myUsuario.Contrasenia;

                    userInfo.Expires.Add(new TimeSpan(0, 1, 0));
                    Response.Cookies.Add(userInfo);
                    //Creo la cookie
                    Response.Cookies.Add(userCookie);

                    HttpContext.Response.Cookies["Email"].Value = email;
                        HttpContext.Response.Cookies["Contrasenia"].Value = Contrasenia;
                        HttpContext.Response.Cookies["Email"].Expires = DateTime.Now.AddMinutes(2);
                        HttpContext.Response.Cookies["Contrasenia"].Expires = DateTime.Now.AddMinutes(2);
                        

                    }
                    else
                    {
                        HttpContext.Response.Cookies["Email"].Expires = DateTime.Now.AddMinutes(-1);
                        HttpContext.Response.Cookies["Contrasenia"].Expires = DateTime.Now.AddMinutes(-1);

                    }

           
                //capturo todos los datos en una sesion
                Session["usuario"] = myUsuario;
                Session["email"] = myUsuario.Email;
                Session["nombre"] = myUsuario.Nombre;
                Session["id"] = myUsuario.IdUsuario;
                Session["contra"] = myUsuario.Contrasenia;


                var usuario = (int)Session["id"];
          }
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