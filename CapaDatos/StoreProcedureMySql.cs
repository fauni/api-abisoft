using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaSeguridad;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CapaDatos
{
    public class GeneracionConexionMySql
    {
        public static string ConexionSQL(string Server, string BaseDatos, string Usuario, string Password)
        {
            String conguardar = "datasource=" + Server + ";port=3306;username=" + Usuario + ";password=" + Password + ";database=" + BaseDatos + "; SslMode=none; Convert Zero Datetime=True";
            return conguardar;
        }
    }

    public class ConsultaMySql
    {

        ManejoErrores log = new ManejoErrores("Api Abisoft");
        // CapaSeguridad.ManejoErrores log = new CapaSeguridad.ManejoErrores("CONEXION MYSQL");
        //Variable que contiene el nombre del Store Procedure
        private string NombreSP = String.Empty;
        private string sql = String.Empty;
        //Variable que contiene el error que se ocaciono
        private string MensajeError = String.Empty;
        //Variable que indica cual es el ultimo id insertado
        private long ultimoIdInsertado = 0;

        public ConsultaMySql(string sql)
        {
            this.sql = sql;
        }

        public string Error
        {
            get { return MensajeError; }
        }

        public DataTable EjecutarConsulta(string CadenaConexion)
        {

            MySqlConnection conexion = new MySqlConnection(CadenaConexion);
            MySqlCommand comando = new MySqlCommand(sql, conexion);
            comando.CommandTimeout = 60;

            DataTable Consulta = new DataTable();
            MySqlDataReader reader;

            int count = 0;
            try
            {
                conexion.Open();
                Consulta.Load(comando.ExecuteReader());
                if (sql.ToUpper().Contains("INSERT INTO"))
                {
                    this.ultimoIdInsertado = comando.LastInsertedId;
                }
                else
                {
                    this.ultimoIdInsertado = 0;
                }
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
                log.RegistroLogError(ex);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conexion.Close();
            }
            return Consulta;
        }
        public int getIdUltimoInsertado()
        {
            return Convert.ToInt32(this.ultimoIdInsertado);
        }
    }

    public class StoreProcedureMySql
    {

    }
}
