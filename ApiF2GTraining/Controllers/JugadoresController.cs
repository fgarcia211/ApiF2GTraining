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

        // POST: api/Jugadores
        /// <summary>
        /// Inserta un jugador introducido por el usuario en la BBDD
        /// </summary>
        /// <remarks>
        /// Inserta jugador en la BBDD:
        /// 
        /// - El ID de equipo debe pertenecer al usuario
        /// </remarks>
        /// <param name="j">Jugador a introducir</param>
        /// <response code="200">OK. Inserta el jugador en la BBDD</response>
        /// <response code="400">ERROR: Ha ocurrido algun error de introduccion</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                //Hay que comprobar el ID posicion, que existe
                await this.repo.InsertJugador(j.IdEquipo, j.IdPosicion, j.Nombre, j.Dorsal, j.Edad, j.Peso, j.Altura);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Jugadores/GetPosiciones
        /// <summary>
        /// Devuelve todas las posiciones de los jugadores disponibles
        /// </summary>
        /// <remarks>
        /// Devuelve las posiciones
        /// </remarks>
        /// <response code="200">OK. Devuelve las posiciones introducidas</response>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Posicion>>> GetPosiciones()
        {
            return await this.repo.GetPosiciones();
        }

        // GET: api/Jugadores/GetJugadorID/{idjugador}
        /// <summary>
        /// Devuelve el jugador con el ID correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve jugador por ID en la BB.DD
        /// 
        /// - El ID de usuario, del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Devuelve el jugador de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el jugador con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        // GET: api/Jugadores/GetEstadisticasJugador/{idjugador}
        /// <summary>
        /// Devuelve las estadisticas del jugador con el ID correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve estadisticas por ID Jugador en la BB.DD
        /// 
        /// - El ID de usuario, del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Devuelve las estadisticas de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el jugador con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        // GET: api/Jugadores/GetJugadoresEquipo/{idequipo}
        /// <summary>
        /// Devuelve los jugadores pertenecientes al ID de equipo introducido en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve jugadores por ID Equipo en la BB:DD
        /// 
        /// - El ID de usuario del equipo, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idequipo">ID de equipo a introducir</param>
        /// <response code="200">OK. Devuelve los jugadores de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el equipo con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        // DELETE: api/Jugadores/DeleteJugador/{idjugador}
        /// <summary>
        /// Borra el jugador con el ID Correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Borra el jugador con ese ID
        /// 
        /// - El ID de usuario ,del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Borra el jugador de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el equipo con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpDelete]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
                    return BadRequest(new
                    {
                        response = "Error: Hay IDS de jugador repetidos"
                    });
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
                        return BadRequest(new
                        {
                            response = "Error: Un jugador introducido no pertenece al equipo del entrenamiento"
                        });
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
            if (entrena == null)
            {
                return NotFound();
            }
            else if (idsjugador.Count() == 0 || valoraciones.Count() == 0)
            {
                return BadRequest(new
                {
                    response = "Error: Debes introducir ids de jugadores y valoraciones"
                });
            }
            else if (entrena.Activo != true)
            {
                return BadRequest(new
                {
                    response = "Error: El entrenamiento no esta activo"
                });
            }
            else if (HelperF2GTraining.HayRepetidos(idsjugador))
            {
                return BadRequest(new
                {
                    response = "Error: Hay IDS de jugador repetidos"
                });
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
                        return BadRequest(new
                        {
                            response = "Error: Debes introducir 6 valoraciones entre 0 y 10 por cada ID jugador"
                        });
                    }
                    else
                    {
                        foreach (int val in valoraciones)
                        {
                            if (val > 10 || val < 0)
                            {
                                return BadRequest(new
                                {
                                    response = "Error: Las valoraciones deben encontrarse entre 0 y 10"
                                });
                            }
                        }

                        await this.repo.AniadirPuntuacionesEntrenamiento(idsjugador, valoraciones, identrenamiento);
                        await this.repo.FinalizarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        response = "Error: No se han introducido los IDs de jugador pertenecientes a ese entrenamiento"
                    });
                }
                
            }
            else
            {
                return Unauthorized();
            }

        }

    }
}
