using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using NLog;
using NLog.Config;
using NLog.Targets;
using System.Net.Mail;
using System.Collections;
using System.Configuration;

namespace CapaSeguridad
{
    public class ManejoErrores
    {
        #region properties
        public string Aplicacion { get; set; }
        public string Directorio { get; set; }
        public string EntradaEventLog { get; set; }
        public string Correo_Aplicacion { get; set; }
        public string Smtp { get; set; }
        public string Destinatario_Correos { get; set; }
        #endregion
        /// <summary>
        /// Contructor de la Clase al que se debe proporcionar los valores de los parametros de los log que se desea tener disponibles
        /// </summary>
        /// <param name="aplicacion">Nombre de la Aplicacion</param>
        /// <param name="directorio">Directorio para la escritura de log en un archivo txt (si no se ingresa no se registrara en ese tipo de log)</param>
        /// <param name="entradaEventLog">Entrada de Eventlog para la escritura de logs(si no se ingresa no se registrara en ese tipo de log)</param>
        /// <param name="correo_aplicacion">Correo del Remitente para envio de correos(si no se ingresa no se enviará correos)</param>
        /// <param name="smtp">Servidor de Correos para utilizar el envio de correos(si no se ingresa no se enviará correos)</param>
        /// <param name="destinatario_correos">Correo destinatario de las alertas(si no se ingresa no se enviará correos)</param>

        string ENTRADA_LOG = "LOG_API";
        string DIRECTORIO_LOG = "C:\\LOG_Api";
        string REMITENTE_CORREOS = "amrfranz@gmail.com";
        string SERVIDOR_CORREOS = "mail.com";
        string DESTINATARIO_CORREOS = "amr_franz@hotmail.com";

        public ManejoErrores(string aplicacion)
        {
            Aplicacion = aplicacion;
            try
            {
                EntradaEventLog = ENTRADA_LOG;// configuration["ENTRADA_LOG"];
            }
            catch (Exception ex)
            {
                EntradaEventLog = "Application";
                RegistroDefaultEventLog("No se tiene el parametro ENTRADA_LOG en el archivo de configuracion: " + ex.Message);
            }
            try
            {
                //if (ConfigurationSettings.AppSettings["DIRECTORIO_LOG"].ToString().Trim().Length > 0)
                if (DIRECTORIO_LOG.Length > 0)
                    // Directorio = ConfigurationSettings.AppSettings["DIRECTORIO_LOG"].ToString() + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
                    Directorio = DIRECTORIO_LOG + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\";
                else
                    Directorio = string.Empty;
            }
            catch (Exception ex)
            {
                Directorio = string.Empty;
                RegistroEventLog(EntradaEventLog, "No se tiene el parametro DIRECTORIO_LOG en el archivo de configuracion: " + ex.Message, EventLogEntryType.Information);
            }
            try
            {
                Correo_Aplicacion = REMITENTE_CORREOS; // ConfigurationSettings.AppSettings["REMITENTE_CORREOS"].ToString();
            }
            catch (Exception ex)
            {
                RegistroEventLog(EntradaEventLog, "No se tiene el parametro REMITENTE_CORREOS en el archivo de configuracion: " + ex.Message, EventLogEntryType.Information);
            }
            try
            {
                Smtp = SERVIDOR_CORREOS; // ConfigurationSettings.AppSettings["SERVIDOR_CORREOS"].ToString();
            }
            catch (Exception ex)
            {
                Smtp = string.Empty;
                RegistroEventLog(EntradaEventLog, "No se tiene el parametro SERVIDOR_CORREOS en el archivo de configuracion: " + ex.Message, EventLogEntryType.Information);
            }
            try
            {
                Destinatario_Correos = DESTINATARIO_CORREOS; // ConfigurationSettings.AppSettings["DESTINATARIO_CORREOS"].ToString();
            }
            catch (Exception ex)
            {
                Destinatario_Correos = string.Empty;
                RegistroEventLog(EntradaEventLog, "No se tiene el parametro DESTINATARIO_CORREOS en el archivo de configuracion: " + ex.Message, EventLogEntryType.Information);
            }
        }
        /// <summary>
        /// Funcion para realizar la escritura del directorio para la escritura de log en archivo de texto
        /// </summary>
        /// <returns>Resultado Creacion Directorio (ruta)</returns>
        private bool CrearArchivoLog()
        {
            if (Directorio.Length > 0)
            {
                if (!Directory.Exists(Directorio))
                {
                    try
                    {
                        Directory.CreateDirectory(Directorio);
                    }
                    catch (Exception ex)
                    {
                        RegistroEventLog(EntradaEventLog, "No se pudo crear el directorio log en la ruta: " + Directorio + " Error: " + ex.Message, EventLogEntryType.Error);
                        return false;
                    }
                }
                string Log = "Log-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(Directorio + Log))
                {
                    try
                    {
                        File.Create(Directorio + Log);
                    }
                    catch (Exception ex)
                    {
                        RegistroEventLog(EntradaEventLog, "No se pudo crear el archivo log en la ruta: " + Directorio + Log + "Error: " + ex.Message, EventLogEntryType.Error);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                RegistroEventLog(EntradaEventLog, "No se tiene una ruta de directorio para escribir un archivo de log", EventLogEntryType.Error);
                return false;
            }
        }
        /// <summary>
        /// Deberia Utilizarce para registrar alertas de la aplicacion pero que no generan un error critico o que requiera una revision o correccion manual
        /// Escribira en el eventlog y en el archivo txt
        /// </summary>
        /// <param name="mensaje">Mensaje a escribir en el log</param>
        public void RegistroLogAlerta(string mensaje)
        {
            RegistroEventLog(EntradaEventLog, mensaje, EventLogEntryType.Warning);
            Registro_ArchivoLog(mensaje, "A");
        }
        /// <summary>
        /// Deberia utilizarce para registrar errores de la aplicacion que son criticos y requieren la atención urgente ya que requiere regularizacion manuales
        /// Escribirpa en el eventlog, archivo txt y generará el envio de correo
        /// </summary>
        /// <param name="mensaje">Mensaje a escribir en el log</param>
        public void RegistroLogError(Exception mensaje)
        {
            RegistroEventLog(EntradaEventLog, mensaje.Message, EventLogEntryType.Error);
            Registro_ArchivoLogError(mensaje);
            enviar_mail(mensaje.Message, "Se genero un error en la aplicacion: " + Aplicacion);
        }
        /// <summary>
        /// Deberia utilizarse para registrar datos informativo de la aplicacion, que sirvan de trace en caso de requerir seguimiento
        /// Esscribirá solamente en el archivo txt
        /// </summary>
        /// <param name="mensaje">Mensaje a escribir en el log</param>
        public void RegistroLogInformacion(string mensaje)
        {
            Registro_ArchivoLog(mensaje, "I");
        }
        /// <summary>
        /// Funcion para el registro de eventos en el event log
        /// </summary>
        /// <param name="entradalog">Nombre del Log</param>
        /// <param name="mensaje">Detalle Mensaje a registrar</param>
        /// <param name="Tipo">Tipo de Alerta con la que se registre</param>
        private void RegistroEventLog(string entradalog, string mensaje, EventLogEntryType Tipo)
        {
            try
            {
                if (!EventLog.SourceExists(entradalog))
                {
                    EventLog.CreateEventSource(entradalog, entradalog);
                }
                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = entradalog;
                myLog.Log = string.Empty;
                // Write an informational entry to the event log.
                myLog.WriteEntry("Log de " + Aplicacion + ": " + mensaje, Tipo);
            }
            catch (Exception ex)
            {
                RegistroDefaultEventLog("No se pudo registrar en el eventlog " + entradalog + " verifique la existencia o permisos de esa entrada. El error generado fue " + ex.Message + ". El error que se trato de registrar fue " + mensaje);
            }
        }
        /// <summary>
        /// Funcion para el registro de eventos en el event log, en caso de error critico se registra en la entrada application
        /// </summary>        
        /// <param name="mensaje">Detalle Mensaje a registrar</param>        
        private void RegistroDefaultEventLog(string mensaje)
        {
            if (!EventLog.SourceExists("Application"))
            {
                EventLog.CreateEventSource("Application", "Application");
            }
            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            myLog.Source = "Application";
            myLog.Log = string.Empty;
            // Write an informational entry to the event log.
            myLog.WriteEntry("Log de " + Aplicacion + ": " + mensaje, EventLogEntryType.Error);
        }
        /// <summary>
        /// Funcion para realizar el registro de eventos en archivo de texto Log-yyyyMMdd.txt
        /// </summary>
        /// <param name="mensaje">Detalle del error a escribir</param>
        private void Registro_ArchivoLog(string mensaje, string tipo)
        {
            string Log = "Log-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            try
            {
                //if (CrearArchivoLog())
                //{                    
                //    File.AppendAllText(Directorio + Log, "Aplicacion: " + Aplicacion + " -> Fecha: " + DateTime.Now.ToString() + " -> Descripcion: " + mensaje + Environment.NewLine);

                //}
                TextLogger archivotexto = new TextLogger(Directorio + Log);
                switch (tipo)
                {
                    case "I":
                        archivotexto.LogInformation(LogManager.GetCurrentClassLogger(), mensaje);
                        break;
                    case "A":
                        archivotexto.LogWarning(LogManager.GetCurrentClassLogger(), mensaje);
                        break;

                }
            }
            catch (Exception ex)
            {
                RegistroEventLog(EntradaEventLog, "No se pudo escribir el log en el archivo de texto " + Log + " en el directorio: " + Directorio + " -> Error: " + ex.Message, EventLogEntryType.Warning);
            }
        }

        private void Registro_ArchivoLogError(Exception mensaje)
        {
            string Log = "Log-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            try
            {
                //if (CrearArchivoLog())
                //{                    
                //    File.AppendAllText(Directorio + Log, "Aplicacion: " + Aplicacion + " -> Fecha: " + DateTime.Now.ToString() + " -> Descripcion: " + mensaje + Environment.NewLine);

                //}
                TextLogger archivotexto = new TextLogger(Directorio + Log);
                archivotexto.LogError(LogManager.GetCurrentClassLogger(), mensaje);
            }
            catch (Exception ex)
            {
                RegistroEventLog(EntradaEventLog, "No se pudo escribir el log en el archivo de texto " + Log + " en el directorio: " + Directorio + " -> Error: " + ex.Message, EventLogEntryType.Warning);
            }
        }
        /// <summary>
        /// Funcion para el envio de correos de alerta de errores
        /// </summary>
        /// <param name="From_Body">Detalle del Mensaje a ser enviado</param>
        /// <param name="Subject">Nombre del Titulo del correo</param>
        private void enviar_mail(string From_Body, string Subject)
        {
            if (Smtp.Length > 0)
            {
                if (Correo_Aplicacion.Length > 0)
                {
                    string From_Email;
                    string From_Name;
                    string To_Email_1;
                    char[] caracteres = { '\'' };
                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage message = new MailMessage();
                    From_Body = EliminarCaracteres(From_Body, caracteres);
                    Subject = EliminarCaracteres(Subject, caracteres);
                    From_Email = Correo_Aplicacion;
                    From_Name = Aplicacion;
                    if (Destinatario_Correos.Trim() != "")
                    {
                        To_Email_1 = Destinatario_Correos;

                        try
                        {
                            MailAddress fromAddress = new MailAddress(From_Email, From_Name);
                            // You can specify the host name or ipaddress of your server
                            // Default in IIS will be localhost
                            smtpClient.Host = Smtp;
                            //Default port will be 25
                            smtpClient.Port = 25;
                            //From address will be given as a MailAddress Object
                            message.From = fromAddress;
                            // To address collection of MailAddress
                            message.To.Add(To_Email_1);
                            message.Subject = Subject;
                            message.IsBodyHtml = false;
                            // Message body content
                            message.Body = From_Body;
                            // Send SMTP mail
                            smtpClient.Send(message);
                        }
                        catch (SmtpException err_mail)
                        {
                            RegistroEventLog(EntradaEventLog, "Se produjo un error al realizar el envio de mail " + err_mail.Message, EventLogEntryType.Error);
                        }

                        catch (Exception ex)
                        {
                            RegistroEventLog(EntradaEventLog, "El email  no pudo ser enviado. " + " Error: " + ex.Message, EventLogEntryType.Error);
                        }
                    }
                    else
                        RegistroEventLog(EntradaEventLog, "El destinario(s) de envio de correos se encuentra vacio, valide sus parametros", EventLogEntryType.Warning);
                }
                else
                    RegistroEventLog(EntradaEventLog, "El remitente de envio de correos se encuentra vacio, valide sus parametros", EventLogEntryType.Warning);
            }
            else
                RegistroEventLog(EntradaEventLog, "El nombre o ip del servidor de correos se encuentra vacio, valide sus parametros", EventLogEntryType.Warning);
        }
        /// <summary>
        /// Funcion para la limpieza de caracteres especoales no aceptados en el subject o body para evitar mensajes no entendibles
        /// </summary>
        /// <param name="Cadena">Cadena de texto</param>
        /// <param name="Caracteres">Valores no aceptados</param>
        /// <returns>Cadena con caracteres eliminados</returns>
        private static string EliminarCaracteres(string Cadena, params char[] Caracteres)
        {
            string Cadena2 = String.Empty;
            for (int indice = 0; indice < Cadena.Length; indice++)
            {
                bool Existe = false;
                foreach (char caracter in Caracteres)
                {
                    if (Cadena[indice].CompareTo(caracter) == 0)
                    {
                        Existe = true;
                        break;
                    }
                }
                if (!Existe)
                    Cadena2 += Cadena[indice];
            }
            return Cadena2;
        }
    }

    public class TextLogger
    {
        LoggingConfiguration config = new LoggingConfiguration();
        FileTarget fileTarget = new FileTarget();

        public TextLogger(string archivo)
        {
            Hashtable ConfiguracionNlog = (Hashtable)ConfigurationManager.GetSection("ConfiguracionNlog");
            //fileTarget.FileName = ConfiguracionNlog["DirecDailyLogs"].ToString() + ".txt";
            //fileTarget.Layout = ConfiguracionNlog["LayoutNlog"].ToString();
            fileTarget.FileName = archivo;
            fileTarget.Layout = "${longdate} ${message}";

            config.AddTarget("logfile", fileTarget);

            LoggingRule rule = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private String getMethodInvoke()
        {
            String metodName = String.Empty;
            try
            {
                StackFrame frame = new StackFrame(4);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;
                metodName = name.ToString();
            }
            catch (Exception)
            {
                metodName = "Error no se pudo obtener el nombre del metodo";
            }
            return metodName;
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Informacion
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="descripcion">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        public void LogInformation(Logger logger, string description)
        {
            try
            {
                logger.Info("Metodo: " + getMethodInvoke() + "   Informacion:    -> " + description);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Informacion
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="descripcion">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        public void LogWarning(Logger logger, string description)
        {
            try
            {
                logger.Info("Metodo: " + getMethodInvoke() + "   Advertencia:       -> " + description);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Error
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="description">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        /// <param name="error">registra la informacion de la excepciones</param>
        public void LogError(Logger logger, Exception error)
        {
            try
            {
                logger.Info("Metodo: " + getMethodInvoke() + "   Error:       -> " + error.ToString());
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Crea un registro en el archivo de texto del log con este registro es de Error
        /// </summary>
        /// <param name="logger">Nombre de la clase que invoco el archivo de configuracion</param>
        /// <param name="description">descripcion del mensaje a registrarse en el archivo de configuracion</param>
        /// <param name="error">registra la informacion de la excepciones</param>
        public void LogError(Logger logger, Exception error, String message)
        {
            try
            {
                StackFrame frame = new StackFrame(1);
                var method = frame.GetMethod();
                var type = method.DeclaringType;
                var name = method.Name;
                logger.Info("Metodo: " + getMethodInvoke() + "   Error:       -> " + error.ToString() + "     Mensaje: " + message);
            }
            catch (Exception)
            {

            }
        }
    }
}
