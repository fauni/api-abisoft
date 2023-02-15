using CapaNegocio;
using CapaModelos;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatoController : ControllerBase
    {
        PlatoNegocio n = new PlatoNegocio();
        // GET: api/<PlatoController>
        [HttpGet]
        public IActionResult Get()
        {
            List<Plato> platos = new List<Plato>();
            platos = n.obtenerPlatosDisponibles();
            if (platos == null)
            {
                return BadRequest("Ocurrio un error en el Servidor");
            }
            else if (platos.Count > 0)
            {
                return Ok(platos);
            }
            else
            {
                return NotFound();
            }
        }

        // GET api/<PlatoController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Plato plato = new Plato();
            plato = n.obtenerPlatosPorId(id);
            if (plato == null)
            {
                return BadRequest("Ocurrio un error en el Servidor");
            }
            else if (plato.id > 0)
            {
                return Ok(plato);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/<PlatoController>
        [HttpPost]
        public IActionResult Post([FromBody] Plato value)
        {
            n.guardarPlato(value);
            if (String.IsNullOrEmpty(n.Error))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Ocurrio un Error:" + n.Error);
            }
        }

        // PUT api/<PlatoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Plato value)
        {
            n.modificarPlato(id, value);
            if (String.IsNullOrEmpty(n.Error))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Ocurrio un Error:" + n.Error);
            }
        }

        // DELETE api/<PlatoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            n.eliminarPlato(id);
            if (String.IsNullOrEmpty(n.Error))
            {
                return Ok();
            }
            else
            {
                return BadRequest("Ocurrio un Error:" + n.Error);
            }
        }
    }
}
