using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpPW.Models
{
    public class DetalleTarea
    {
        public Tarea Tarea { get; set; }
        public List<ComentarioTarea> ListaComentarioTarea = new List<ComentarioTarea>();
        public List<ArchivoTarea> ListaArchivoTarea = new List<ArchivoTarea>();
        public ComentarioTarea NuevoComentario = new ComentarioTarea();
    }
}