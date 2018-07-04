﻿using System;
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
using TpPW.Models.Entities;



namespace TpPW.Controllers
{
    public class UsuarioController : Controller
    {

        public TareasEntities context = new TareasEntities();

        private CarpetaServicio CarpetaSer = new CarpetaServicio();





        //Creo un nuevo usuario
        //Tomo los valores
        [HttpGet]
        public ActionResult NuevoUsuario()
        {
            return View();
        }


        //Guardo en la base
        [HttpPost]
        public ActionResult NuevoUsuario(Usuario usuario)
        {
            var encodedResponse = Request.Form["g-Recaptcha-Response"];
            var isCaptchaValid = Captcha.Validate(encodedResponse);

            if (!isCaptchaValid)
            {
                ViewBag.Messege = "El captcha es inválido";
            }
            else
            {
                var Usu = (from u in context.Usuario select u).ToList();

                string NombreUsu = usuario.Nombre;

                string ApeUsu = usuario.Apellido;

                string EmailUsu = usuario.Email;

                string Contra = usuario.Contrasenia;

                string ActivoUsu = Convert.ToString(usuario.Activo);
                usuario.Activo = Convert.ToInt16(ActivoUsu);


                usuario.FechaRegistracion = DateTime.Now;

                usuario.FechaActivacion = DateTime.Now;

                usuario.CodigoActivacion = "4AE52B1C-C3E2-4AB1-8EFD-859FCB87F5B4";



                // hay que verificar que el email no existe
                // hay que validar si la cuenta esta activa o no
                Carpeta car = new Carpeta();

                if (usuario.Contrasenia == usuario.ContraseniaConfirmacion)
                {


                    if (VerificoEmail(EmailUsu) == false)
                    {


                        usuario.Activo = 1;


                        context.Usuario.Add(usuario);
                        context.SaveChanges();


                        //Tiene que crear una nueva carpeta con nombre gral. referiada a ese usuario
                        car.Nombre = "General";
                        car.FechaCreacion = DateTime.Now.Date;
                        car.Descripcion = "Carpeta creada por default";
                        car.IdUsuario = usuario.IdUsuario;

                        context.Carpeta.Add(car);
                        context.SaveChanges();



                        Session["usuario"] = usuario;
                        Session["contra"] = Contra;
                        Session["nombre"] = NombreUsu;
                        Session["email"] = EmailUsu;
                        Session["id"] = usuario.IdUsuario;

                        //cambiar redireccionamiento
                        return RedirectToAction("../Home/Home");


                    }
                    else
                    {
                        if (VerificoEmail(EmailUsu) == true)
                        {


                            //Verifico si el usuario esta activo o no
                            if (VerificoActividad(ActivoUsu) == true)
                            {


                                EmailExisteInactivo(usuario);
                                //CarpetaSer.CreoCarpetaNuevoUsuario(usuario);



                                return RedirectToAction("../Home/Home");


                            }
                            else
                            {
                                ViewBag.Messege = "El email ya existe";
                                return RedirectToAction("NuevoUsuario");

                            }

                        }

                    }
                }
            }

            ViewBag.Mensaje = "Algo salio mal";
            return RedirectToAction("NuevoUsuario");
        }



        //Valido el email.
        public bool VerificoEmail(string email)
        {

            return context.Usuario.Any(x => x.Email == email);
        }

        // ver esta validacion
        public bool VerificoActividad(string activo)
        {
            return context.Usuario.Any(x => x.Activo == 1);

        }






        //Sobre escrivo un usuario existente
        public void EmailExisteInactivo(Usuario usuario)
        {

           

            //consulto el usuario que contiene ese email
            Usuario usu = (from u in context.Usuario where u.Email.Equals(usuario.Email) select u).First();

            Carpeta car = new Carpeta();

            usu.Nombre = usuario.Nombre;
            usu.Apellido = usuario.Apellido;
            usu.Contrasenia = usuario.Contrasenia;
            usu.ContraseniaConfirmacion = usuario.Contrasenia;
            usu.FechaRegistracion= usuario.FechaRegistracion;
            usu.FechaActivacion = usuario.FechaActivacion;
            usu.CodigoActivacion = usuario.CodigoActivacion;
            usu.Activo = 1;
            //usuario.IdUsuario = usu.IdUsuario;

            context.Entry(usu).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();

            //y no me sale este punto fuck
            //Tiene que crear una nueva carpeta con nombre gral. referiada a ese usuario
            car.Nombre = "General";
            car.FechaCreacion = DateTime.Now.Date;
            car.Descripcion = "Carpeta creada por default";
            car.IdUsuario = usuario.IdUsuario;

                        

            usu.Carpeta.Add(car);            

            context.Carpeta.Add(car);

            context.SaveChanges();


            //registro la sesion
            Session["id"] = usu.IdUsuario;
            Session["usuario"] = usu;


        }

    }
}