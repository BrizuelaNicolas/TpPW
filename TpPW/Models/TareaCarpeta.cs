using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TpPW.Models
{
    public class TareaCarpeta
    {
        public Tarea tarea { get; set; }
        List<Carpeta> carpeta = new List<Carpeta>();
    }
}