//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TpPW.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ArchivoTarea
    {
        public int IdArchivoTarea { get; set; }

        [Display(Name = "Ruta")]
        public string RutaArchivo { get; set; }

        public int IdTarea { get; set; }

        [Display(Name ="Fecha de Creacion")]
        public System.DateTime FechaCreacion { get; set; }
   
        public virtual Tarea Tarea { get; set; }
    }
}
