using Stressless_Service.Models;

namespace Stressless_Service.JwtSecurityTokens
{
    public interface IAuthenticationController
    {
        AuthenticationTokenModel Authenticate(string macAddres, string clientID);
        Task<int> CheckPreviousAuth(string macAddress, string id, int returnValue = -1);
    }
}
