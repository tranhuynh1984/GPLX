using GPLX.Core.DTO.Request;
using GPLX.Core.DTO.Response;
using GPLX.Database.Models;
using System.Threading.Tasks;
using GPLX.Core.DTO.Response.Users;

namespace GPLX.Core.Contracts
{
    public interface IMedAuthenticateConnect
    {
        Task<string> GetAccessTokenAsync(string endPoint, MedAccesstokenRequest request);
        Task<MedApiResponse<UserSync>> LoginAsync(string accesstoken, string user, string pass, string endPoint);
        Task<MedApiResponse<Units>> GetUnitsAsync(string endPoint, string accesstoken);
        Task<MedApiResponse<UserSync>> GetUsersAsync(string endPoint, string accesstoken, int page, int size);
        Task<MedApiResponse<Users>> GetUserByUnitCodeAsync(string code, string accesstoken, string endPoint);
        Task<MedTelegramBotResponse> SendMessage(string body, string accesstoken, string endPoint);
    }
}
