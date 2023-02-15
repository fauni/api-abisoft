using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class ELog
    {
        public CapaSeguridad.ManejoErrores log = new CapaSeguridad.ManejoErrores("ABISOFT");

        
        //Variable que contiene el error que se ocaciono
        public string MensajeError = String.Empty;
        /// <summary>
        /// Atributo Error generado despues de ejecutar un método
        /// </summary>
        public string Error
        {
            get { return MensajeError; }
            set { }
        }
    }
}
