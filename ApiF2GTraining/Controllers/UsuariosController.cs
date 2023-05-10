using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using F2GTraining.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiF2GTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private IRepositoryF2GTraining repo;
        private HelperOAuthToken helper;

        public UsuariosController(IRepositoryF2GTraining repo, HelperOAuthToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        // GET: api/Usuarios
        /// <summary>
        /// Inserta un usuario en la BB.DD de USUARIOS
        /// </summary>
        /// <remarks>
        /// Inserta usuarios en la BB.DD
        /// </remarks>
        /// <param name="user">JSON del usuario</param>
        /// <response code="200">OK. Devuelve los entrenamientos del equipo solicitado</response>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> InsertUsuario(Usuario user)
        {
            await this.repo.InsertUsuario(user);
            return Ok();
        }

        // POST: api/Usuarios/Login/{nombre}/{contrasenia}
        /// <summary>
        /// Devuelve el token para hacer login con el nombre y la contrasenia introducida si coincide con la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve token para peticiones
        /// </remarks>
        /// <param name="nombre">Nombre del usuario</param>
        /// <param name="contrasenia">Contraseña del usuario</param>
        /// <response code="200">OK. Devuelve el token para realizar peticiones protegidas</response>        
        /// <response code="401">Credenciales incorrectas</response>
        [HttpPost]
        [Route(("[action]/{nombre}/{contrasenia}"))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Usuario>> Login(string nombre, string contrasenia)
        {
            Usuario user = await this.repo.GetUsuarioNamePass(nombre, contrasenia);
            if (user != null)
            {
                SigningCredentials credentials =
                new SigningCredentials(this.helper.GetKeyToken()
                , SecurityAlgorithms.HmacSha256);

                string jsonUser = JsonConvert.SerializeObject(user);
                Claim[] info = new[]
                {
                    new Claim("UserData", jsonUser)
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: info,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(180),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Usuarios/TelefonoRegistrado/{telefono}
        /// <summary>
        /// Devuelve si un telefono introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un telefono ya existe
        /// </remarks>
        /// <param name="telefono">Telefono a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{telefono}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> TelefonoRegistrado(int telefono)
        {
            return await this.repo.CheckTelefonoRegistro(telefono);
        }

        // GET: api/Usuarios/NombreRegistrado/{nombre}
        /// <summary>
        /// Devuelve si un nombre introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un nombre ya existe
        /// </remarks>
        /// <param name="nombre">Nombre a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{nombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> NombreRegistrado(string nombre)
        {
            return await this.repo.CheckUsuarioRegistro(nombre);
        }

        // GET: api/Usuarios/CorreoRegistrado/{correo}
        /// <summary>
        /// Devuelve si un correo introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un correo ya existe
        /// </remarks>
        /// <param name="correo">Correo a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{correo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CorreoRegistrado(string correo)
        {
            return await this.repo.CheckCorreoRegistro(correo);
        }

    }
}
