using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpPW.Models
{
    public class TareaComentarioArchivo
    {
        public Tarea tarea { get; set; }
        public ComentarioTarea comentario { get; set; }
        public ArchivoTarea archivo { get; set; }
    }
}