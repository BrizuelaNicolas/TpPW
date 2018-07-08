using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TpPW.Models
{
    public class ArchivoModelView
    {
        public static string Guardar(HttpPostedFileBase archivoSubido, string nombreSignificativo, string carpeta)
        {
            //Server.MapPath antepone a un string la ruta fisica donde actualmente esta corriendo la aplicacion (ej. c:\inetpub\misitio\)
            string pathDestino = System.Web.Hosting.HostingEnvironment.MapPath("~") + carpeta;

            //si no exise la carpeta, la creamos
            if (!System.IO.Directory.Exists(pathDestino))
            {
                System.IO.Directory.CreateDirectory(pathDestino);
            }
            string fileExtension = nombreSignificativo.Split('.').LastOrDefault();
            string fileName = nombreSignificativo.Split('.').FirstOrDefault();
            string nombreArchivoFinal = GenerarNombreUnico(fileName);
            nombreArchivoFinal = string.Concat(nombreArchivoFinal, Path.GetExtension(archivoSubido.FileName));

            //para guardar en el disco rigido, se guarda con el path absoluto
            archivoSubido.SaveAs(string.Concat(pathDestino, nombreArchivoFinal));
            //retornamos el path relativo desde la raiz del sitio
            return string.Concat(carpeta, nombreArchivoFinal);
        }

        private static string GenerarNombreUnico(string nombreSignificativo)
        {
            return $"{nombreSignificativo}_{Guid.NewGuid().ToString()}";
        }

        /// <summary>
        /// Borra la imagen guardada en el server basandose en el parametro (relativo o absoluto)
        /// </summary>
        /// <param name="pathGuardado"></param>
        /// <returns></returns>
        public static void Borrar(string pathGuardado)
        {
            //si el path es relativo, se le agrega el mapeo completo para que sea absoluto
            //y pasar de /temp/imagen.jpg a c:\inetpub\temp\imagen.jpg por ejemplo
            if (Path.GetPathRoot(pathGuardado).Contains(":"))
            {
                //Alternativa a Server.MapPath(
                pathGuardado = System.Web.Hosting.HostingEnvironment.MapPath("~") + pathGuardado;
            }

            if (System.IO.File.Exists(pathGuardado))
            {
                System.IO.File.Decrypt(pathGuardado);
            }
        }
    }
}