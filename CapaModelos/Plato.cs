using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelos
{
    public class Plato
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public DateTime fechaInicioActividad { get; set; }
        public string? color { get; set; }
        public decimal precio { get; set; }
        public string? oferta { get; set; }
        public string? estado { get; set; }
    }
}
