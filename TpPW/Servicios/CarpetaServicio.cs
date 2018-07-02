using TpPW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace TpPW.Servicios
{
    public class CarpetaServicio
    {
        public TareasEntities context = new TareasEntities();

        public void CreoCarpetaNuevoUsuario(Usuario usurio)
        {
            var carpe = (from c in context.Carpeta where c.IdUsuario == usurio.IdUsuario select c).First();

            Carpeta car = new Carpeta();
            
                     
         context.Carpeta.Add(car);
         context.SaveChanges();

            
        }
    }
}