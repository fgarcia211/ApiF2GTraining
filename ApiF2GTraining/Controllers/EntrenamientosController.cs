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
    public class EntrenamientosController : ControllerBase
    {
        private IRepositoryF2GTraining repo;

        public EntrenamientosController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        // POST: api/Entrenamientos
        /// <summary>
        /// Inserta un entrenamiento en la BB.DD segun el nombre del entrenamiento y el ID de su equipo
        /// </summary>
        /// <remarks>
        /// Inserta entrenamiento con nombre, y el id del equipo
        /// </remarks>
        /// <param name="idequipo">Id del equipo.</param>
        /// <param name="nombre">Nombre del entrenamiento</param>
        /// <response code="200">OK. Inserta el entrenamiento en BB.DD</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>  
        [Authorize]
        [HttpPost("{idequipo}/{nombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> InsertEntrenamiento(int idequipo, string nombre)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (user.IdUsuario == equipo.IdUsuario)
            {
                await this.repo.InsertEntrenamiento(idequipo, nombre);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Entrenamientos/GetEntrenamientosEquipo/{idequipo}
        /// <summary>
        /// Busca los entrenamientos, segun el ID de su equipo
        /// </summary>
        /// <remarks>
        /// Busca entrenamientos por ID de equipo
        /// </remarks>
        /// <param name="idequipo">Id del equipo.</param>
        /// <response code="200">OK. Devuelve los entrenamientos del equipo solicitado</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun equipo con ese ID</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Entrenamiento>>> GetEntrenamientosEquipo(int idequipo)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (user.IdUsuario == equipo.IdUsuario && equipo != null)
            {
                return await this.repo.GetEntrenamientosEquipo(idequipo);
            }
            else if (equipo == null)
            {
                return NotFound();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Entrenamientos/GetEntrenamiento/{idequipo}
        /// <summary>
        /// Busca el entrenamiento correspondiente a su ID de entrenamiento
        /// </summary>
        /// <remarks>
        /// Busca entrenamiento por su ID
        /// </remarks>
        /// <param name="identrena">Id del entrenamiento.</param>
        /// <response code="200">OK. Devuelve el entrenamiento solicitado</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun entrenamiento con ese ID</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]/{identrena}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entrenamiento>> GetEntrenamiento(int identrena)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrena);

            if (entrena != null)
            {
                Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

                if (equipo.IdUsuario == user.IdUsuario)
                {
                    return entrena;
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }

        // GET: api/Entrenamientos/GetEntrenamiento/{idequipo}
        /// <summary>
        /// Borra el entrenamiento correspondiente a su ID de entrenamiento
        /// </summary>
        /// <remarks>
        /// Borra entrenamiento por su ID
        /// </remarks>
        /// <param name="identrenamiento">Id del entrenamiento.</param>
        /// <response code="200">OK. Borra el entrenamiento solicitado</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun entrenamiento con ese ID</response> 
        [Authorize]
        [HttpDelete("{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> BorrarEntrenamiento(int identrenamiento)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);

            if (entrena != null)
            {
                Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

                if (equipo.IdUsuario == user.IdUsuario)
                {
                    await this.repo.BorrarEntrenamiento(identrenamiento);
                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
            
        }

    }
}
