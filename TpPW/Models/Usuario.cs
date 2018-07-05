//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;


namespace TpPW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.Carpeta = new HashSet<Carpeta>();
            this.Tarea = new HashSet<Tarea>();
        }
    
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El {0} no puede superar los {1} caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El {0} no puede superar los {1} caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "{0} es obligatorio")]
        [MaxLength(200, ErrorMessage = "El {0} no puede superar los {1} caracteres")]
        [RegularExpression("^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@(([a-zA-Z]+[\\w-]+\\.){1,2}[a-zA-Z]{2,4})$", ErrorMessage = "Email incorrecto")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Es obligatorio")]
        [MaxLength(20, ErrorMessage = "No puede superar los {1} caracteres")]
        // [MinLength(5, ErrorMessage = "{0} Tiene que tener m�nimo {1} caracteres")]
        [RegularExpression("^(?=\\w*\\d)(?=\\w*[A-Z])(?=\\w*[a-z])\\S{0,20}$", ErrorMessage = "Debe tener un maximo de 20 caracteres, aun que sea una Mayusculas una minuscula y un numero")]
        public string Contrasenia { get; set; }

        [Required(ErrorMessage = "Es obligatorio")]       
        [MaxLength(20, ErrorMessage = "No puede superar los {1} caracteres")]
        // [MinLength(5, ErrorMessage = "{0} Tiene que tener m�nimo {1} caracteres")]
        [RegularExpression("^(?=\\w*\\d)(?=\\w*[A-Z])(?=\\w*[a-z])\\S{0,20}$", ErrorMessage = "Debe tener un maximo de 20 caracteres, aun que sea una Mayusculas una minuscula y un numero")]
        [Compare("Contrasenia", ErrorMessage = "Las contrase�as ingresadas no son identicas")]
        //Agrego para comparacion de contrase�as
        public string ContraseniaConfirmacion { get; set; }

        public short Activo { get; set; }

        public System.DateTime FechaRegistracion { get; set; }
        public Nullable<System.DateTime> FechaActivacion { get; set; }
        public string CodigoActivacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carpeta> Carpeta { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tarea> Tarea { get; set; }
    }
}
