using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class Parametros
    {
        #region Conexiones MYSQL ABISOFT
        public static string ConexionBDMySQL()
        {
            try
            {
                //string pass = CapaSeguridad.WS_SegNet.DesEncriptarValor("cWwy4iu9AsKSyGL8JKXaFw==");
                return CapaDatos.GeneracionConexionMySql.ConexionSQL("localhost", "bdabisoft", "root", "");
                // return CapaDatos.GeneracionConexionMySql.ConexionSQL("localhost", "gghh", "bdplato", pass);
            }
            catch (Exception ex)
            {

                throw new Exception("Error en parametros de cadena de conexion de aplicacion: " + ex.Message);
            }
        }

        
        #endregion
    }
}
