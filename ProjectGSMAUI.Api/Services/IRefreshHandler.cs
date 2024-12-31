namespace ProjectGSMAUI.Api.Services
{
    public interface IRefreshHandler
    {
        Task<string> GenerateToken(string username);
    }
}
