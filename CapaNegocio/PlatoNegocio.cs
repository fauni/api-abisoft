using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaModelos;
using CapaDatos;

namespace CapaNegocio
{
    public class PlatoNegocio: ELog
    {
        public List<Plato> obtenerPlatosDisponibles()
        {
            List<Plato> platos = new List<Plato>();
            String sql = @$"select * from plato where estado = 'Activo' and (date(fechaInicioActividad) >= date(now()));";
            try
            {
                ConsultaMySql consulta = new ConsultaMySql(sql);
                DataTable dt = consulta.EjecutarConsulta(Parametros.ConexionBDMySQL());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        Plato p = new Plato();
                        p.id = Convert.ToInt32(item["id"]);
                        p.nombre = (item["nombre"]).ToString();
                        p.fechaInicioActividad = Convert.ToDateTime(item["fechaInicioActividad"]);
                        p.color = (item["color"]).ToString();
                        p.precio = Convert.ToDecimal(item["precio"]);
                        p.oferta = (item["oferta"]).ToString();
                        platos.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                log.RegistroLogError(ex);
                MensajeError = ex.Message;
            }

            return platos;
        }

        public Plato obtenerPlatosPorId(int id)
        {
            Plato plato = new Plato();
            String sql = @$"select * from plato where id = {id};";

            try
            {
                ConsultaMySql consulta = new ConsultaMySql(sql);
                DataTable dt = consulta.EjecutarConsulta(Parametros.ConexionBDMySQL());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        Plato p = new Plato();
                        p.id = Convert.ToInt32(item["id"]);
                        p.nombre = (item["nombre"]).ToString();
                        p.fechaInicioActividad = Convert.ToDateTime(item["fechaInicioActividad"]);
                        p.color = (item["color"]).ToString();
                        p.precio = Convert.ToDecimal(item["precio"]);
                        p.oferta = (item["oferta"]).ToString();
                        plato = p;
                    }
                }
            }
            catch (Exception ex)
            {
                log.RegistroLogError(ex);
                MensajeError = ex.Message;
            }

            return plato;
        }

        public void guardarPlato(Plato plato)
        {
            String sql = @$"insert into plato (nombre, fechaInicioActividad, color, precio, oferta) 
                values ('{plato.nombre}', '{plato.fechaInicioActividad}', '{plato.color}', {plato.precio}, '{plato.oferta}');";
            try
            {
                ConsultaMySql consulta = new ConsultaMySql(sql);
                DataTable dt = consulta.EjecutarConsulta(Parametros.ConexionBDMySQL());

                if (!String.IsNullOrEmpty(consulta.Error))
                {
                    throw new Exception(consulta.Error);
                }
            }
            catch (Exception ex)
            {
                log.RegistroLogError(ex);
                MensajeError = ex.Message;
            }
        }

        public void modificarPlato(int id, Plato plato)
        {
            String sql = @$"update plato set nombre = '{plato.nombre}', fechaInicioActividad = '{plato.fechaInicioActividad}', color= '{plato.color}', 
                precio={plato.precio}, oferta = '{plato.oferta}' where id = {id}; ";
            try
            {
                ConsultaMySql consulta = new ConsultaMySql(sql);
                DataTable dt = consulta.EjecutarConsulta(Parametros.ConexionBDMySQL());

                if (!String.IsNullOrEmpty(consulta.Error))
                {
                    throw new Exception(consulta.Error);
                }
            }
            catch (Exception ex)
            {
                log.RegistroLogError(ex);
                MensajeError = ex.Message;
            }
        }

        public void eliminarPlato(int id)
        {
            String sql = @$"update plato set estado = 'Eliminado' where id = {id};";
            try
            {
                ConsultaMySql consulta = new ConsultaMySql(sql);
                DataTable dt = consulta.EjecutarConsulta(Parametros.ConexionBDMySQL());

                if (!String.IsNullOrEmpty(consulta.Error))
                {
                    throw new Exception(consulta.Error);
                }
            }
            catch (Exception ex)
            {
                log.RegistroLogError(ex);
                MensajeError = ex.Message;
            }
        }
    }
}
