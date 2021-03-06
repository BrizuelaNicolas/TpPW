﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using TpPW.Models;
using System.Web.Security;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using TpPW.Controllers;





namespace TpPW.Controllers
{
    public class CarpetaController : Controller
    {
        public TareasEntities context = new TareasEntities();


        //Listamos Las carpetas
        public ActionResult MisCarpetas()
        {
            //SI existe la cookies que se cargue
            if (Request.Cookies["CookieUsuario"] != null)
            {
                var usuario = Convert.ToUInt32(Session["id"]);

                var carpeta = (from p in context.Carpeta
                               where p.IdUsuario == usuario
                               orderby p.Nombre ascending
                               select p).ToList();

                return View(carpeta);
            }
            else // si no existe cookies, que verifique session
            {
                if (Session["id"] == null)//Si el usuario no esta logueado
                {
                    ViewBag.userNameInSession = "El usuario no esta en la session";
                    Session["returnPath"] = Request.RawUrl;
                    return RedirectToAction("Login", "Home");
                }

                if (Session["usuario"] != null)
                {
                    var usuario = Convert.ToUInt32(Session["id"]);

                    var carpeta = (from p in context.Carpeta
                                   where p.IdUsuario == usuario
                                   orderby p.Nombre ascending
                                   select p).ToList();

                    return View(carpeta);
                }
                else
                {
                    ViewBag.MensajeError = "Usuario o contraseña invalido";
                    return RedirectToAction("../Home/Login");
                }


            }
        }



   [HttpGet]
        public ActionResult NuevaCarpeta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NuevaCarpeta(Carpeta carpeta)
        {
            

            if (ModelState.IsValid)
            {
                if (Session["id"] == null)//Si el usuario no esta logueado
                {
                    ViewBag.userNameInSession = "El usuario no esta en la session";
                    Session["returnPath"] = Request.RawUrl;
                    return RedirectToAction("Login", "Home");
                }
                if (Session["usuario"] != null)
                {
                    
                    string NombreCarp = carpeta.Nombre;

                    string DescripcionCar = carpeta.Descripcion;

                    carpeta.FechaCreacion = DateTime.Now.Date;


                    var usuario = Convert.ToInt32(Session["id"]);

                    var car = (from p in context.Carpeta
                               where p.IdUsuario == usuario
                               orderby p.FechaCreacion
                               select p).ToList();

                    int IdUsu = usuario;

                    carpeta.IdUsuario = IdUsu;
                                       
                    context.Carpeta.Add(carpeta);
                    context.SaveChanges();
                }
                if (carpeta != null)
                {
                    
                    ViewBag.Mensaje =  "Carpeta Creada";
                    return RedirectToAction("MisCarpetas");
                }

                else
                {
                    ViewBag.Mensaje = "La carpeta no pudo ser creada";
                    return View();
                }

            }

            else
            {
                ViewBag.Mensaje = "La carpeta no pudo ser creada";
                return View();
            }


        }

        public ActionResult TareasDeCarpeta(int Id)
        {

            List<Tarea> t = (from p in context.Tarea where Id == p.IdCarpeta orderby p.FechaCreacion select p).ToList();
            return View(t);

        }

    }
}