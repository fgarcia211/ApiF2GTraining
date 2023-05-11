using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using F2GTraining.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiF2GTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JugadoresController : ControllerBase
    {
        private IRepositoryF2GTraining repo;

        public JugadoresController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> InsertJugador(Jugador j)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(j.IdEquipo);

            if (equipo == null)
            {
                return BadRequest();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                await this.repo.InsertJugador(j.IdEquipo, j.IdPosicion, j.Nombre, j.Dorsal, j.Edad, j.Peso, j.Altura);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Posicion>>> GetPosiciones()
        {
            return await this.repo.GetPosiciones();
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        public async Task<ActionResult<Jugador>> GetJugadorID(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return jugador;
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        public async Task<ActionResult<EstadisticaJugador>> GetEstadisticasJugador(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.GetEstadisticasJugador(idjugador);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        public async Task<ActionResult<List<Jugador>>> GetJugadoresEquipo(int idequipo)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (equipo == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.GetJugadoresEquipo(idequipo);
            }
            else
            {
                return Unauthorized();
            }

        }

        [Authorize]
        [HttpDelete]
        [Route("[action]/{idjugador}")]
        public async Task<ActionResult> DeleteJugador(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                await this.repo.DeleteJugador(idjugador);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Jugador>>> JugadoresXUsuario()
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            return await this.repo.JugadoresXUsuario(user.IdUsuario);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{identrenamiento}")]
        public async Task<ActionResult<List<Jugador>>> JugadoresXSesion(int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (entrena == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.JugadoresXSesion(identrenamiento);
            }
            else
            {
                return Unauthorized();
            }

        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{identrenamiento}")]
        public async Task<ActionResult<List<JugadorEntrenamiento>>> GetNotasSesion(int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (entrena == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.GetNotasSesion(identrenamiento);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{identrenamiento}")]
        public async Task<ActionResult> AniadirJugadoresSesion([FromQuery] List<int> idsjugador, int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (entrena == null)
            {
                return BadRequest();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                //HAY QUE COMPROBAR QUE TODOS LOS IDSJUGADOR SEAN DISTINTOS
                if (HelperF2GTraining.HayRepetidos(idsjugador))
                {
                    return BadRequest();
                }
                else
                {
                    List<Jugador> jugadoresseleccionados = new List<Jugador>();
                    foreach (int idjug in idsjugador)
                    {
                        jugadoresseleccionados.Add(await this.repo.GetJugadorID(idjug));
                    }

                    if (HelperF2GTraining.JugadoresEquipoCorrecto(jugadoresseleccionados, equipo.IdEquipo))
                    {
                        await this.repo.AniadirJugadoresSesion(idsjugador, identrenamiento);
                        await this.repo.EmpezarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                }
                
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("[action]/{identrenamiento}")]
        public async Task<ActionResult> AniadirPuntuacionesEntrenamiento([FromQuery] List<int> idsjugador, [FromQuery] List<int> valoraciones, int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            //Comprobamos que el entrenamiento exista, que este activo, y que el usuario haya pasado al menos 1 idjugador y 1 valoracion
            //Tambien comprobamos que no haya repetidos
            if (entrena == null || idsjugador.Count() == 0 || valoraciones.Count() == 0 || entrena.Activo != true || HelperF2GTraining.HayRepetidos(idsjugador))
            {
                return BadRequest();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                //Recogemos los jugadores que estaban apuntados a la sesion
                List<Jugador> jugadoresentrena = await this.repo.JugadoresXSesion(identrenamiento);

                //Comprobamos que los ID sean iguales a los que estan registrados en el entrenamiento
                if (HelperF2GTraining.ComprobarIDJugadoresEntrena(idsjugador, jugadoresentrena))
                {
                    double comprobante = double.Parse(valoraciones.Count().ToString()) / double.Parse(idsjugador.Count().ToString());
                    if (comprobante != 6)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        foreach (int val in valoraciones)
                        {
                            if (val > 10 || val < 0)
                            {
                                return BadRequest();
                            }
                        }

                        await this.repo.AniadirPuntuacionesEntrenamiento(idsjugador, valoraciones, identrenamiento);
                        await this.repo.FinalizarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                }
                else
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return Unauthorized();
            }

        }

    }
}
