using Microsoft.AspNetCore.Mvc;
using ProjectGSMAUI.Api.Helper;
namespace ProjectGSMAUI.Api.Services
{
    public interface IGeminiServices
    {
        Task<APIResponse> TraLoi(string userInput);
    }
}
