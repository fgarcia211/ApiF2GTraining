using F2GTraining.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiF2GTraining.Helpers
{
    public static class HelperContextUser
    {
        public static Usuario GetUsuarioByClaim(Claim claim)
        {
            return JsonConvert.DeserializeObject<Usuario>(claim.Value);
        }
    }
}
