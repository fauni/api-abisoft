using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaSeguridad
{
    public class WS_SegNet
    {
        /// <summary>
        /// Metodo que permite encriptar un valor para enviar al servicio WS_SegNet
        /// </summary>
        /// <param name="ValorAEncriptar">Valor a Encriptar</param>
        /// <returns>Valor Encriptado</returns>
        public static string EncriptarValor(string ValorAEncriptar)
        {
            //Aqui guardamos la clave con la que vamos a cifrar
            byte[] arrayClave;
            //Aqui guardamos el texto a cifrar
            byte[] arregloDeInformacion = UTF8Encoding.UTF8.GetBytes(ValorAEncriptar);
            //Se utiliza System.Security.Cryptography para MD5 
            MD5CryptoServiceProvider variableHashMD5 = new MD5CryptoServiceProvider();
            //Se hace el hash con la clave
            arrayClave = variableHashMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes("Abi2023"));
            variableHashMD5.Clear();
            TripleDESCryptoServiceProvider cifradoTripleDES = new TripleDESCryptoServiceProvider();
            cifradoTripleDES.Key = arrayClave;
            cifradoTripleDES.Mode = CipherMode.ECB; //Especifica el modo de cifrado de bloques que se utilizará para cifrar
            cifradoTripleDES.Padding = PaddingMode.PKCS7; //Especifica el tipo de relleno que se aplica cuando el bloque de datos del mensaje es más pequeño que el número total de bytes necesarios para una operación criptográfica.
            ICryptoTransform cTransform = cifradoTripleDES.CreateEncryptor();
            byte[] resultadoEncriptacion = cTransform.TransformFinalBlock(arregloDeInformacion, 0, arregloDeInformacion.Length);
            cifradoTripleDES.Clear();
            string resultado = Convert.ToBase64String(resultadoEncriptacion, 0, resultadoEncriptacion.Length).Replace("/", "¬");
            return resultado;
        }
        /// <summary>
        /// Metodo que permite desencriptar un valor del servicio WS_SegNet
        /// </summary>
        /// <param name="ValorEncriptado">Valor a Desencriptar</param>
        /// <returns>Valor Desencriptado</returns>
        public static string DesEncriptarValor(string ValorEncriptado)
        {
            byte[] arrayClave;
            byte[] arregloDeInformacinoEncriptada = Convert.FromBase64String(ValorEncriptado.Replace("¬", "/"));
            MD5CryptoServiceProvider variableHashMD5 = new MD5CryptoServiceProvider();
            arrayClave = variableHashMD5.ComputeHash(UTF8Encoding.UTF8.GetBytes("Abi2023"));
            variableHashMD5.Clear();
            TripleDESCryptoServiceProvider cifradoTripleDES = new TripleDESCryptoServiceProvider();
            cifradoTripleDES.Key = arrayClave;
            cifradoTripleDES.Mode = CipherMode.ECB;
            cifradoTripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = cifradoTripleDES.CreateDecryptor();
            byte[] resultadoDesencriptacion = cTransform.TransformFinalBlock(arregloDeInformacinoEncriptada, 0, arregloDeInformacinoEncriptada.Length);
            cifradoTripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultadoDesencriptacion);
        }

        /// <summary>
        /// Función para encriptar y desencriptar valores - Segurinet
        /// </summary>
        /// <param name="EnDec">true - Encriptar, false - Desencriptar</param>
        /// <param name="InputString">String Valor</param>
        /// <param name="OutputRaw">Resultado</param>
        /// <param name="Length">Tamaño del string destino.</param>
        /// <returns></returns>
        //[DllImport("SegCrypt_MiCreditoOrq.dll")]
        //unsafe private static extern bool EncryptDecrypt(bool EnDec, string InputString, byte* OutputRaw, int* Length);
    }
}
