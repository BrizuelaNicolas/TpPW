using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpPW.Models
{
    public class TareaRepositorio
    {
        public TareasEntities context = new TareasEntities();

        public Tarea buscarPorIdTarea(int id)
        {
            Tarea tarea = context.Tarea.Include("ArchivoTarea").FirstOrDefault(x => x.IdTarea == id);
            if (tarea == null)
            {
                throw new Exception("Id de tarea inexistente");
            }
            return tarea;
        }



        internal void AgregarArchivo(int idTarea, string filePath)
        {
            Tarea tarea = buscarPorIdTarea(idTarea);
            tarea.ArchivoTarea.Add(new ArchivoTarea()
            {
                RutaArchivo = filePath,
                FechaCreacion = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}