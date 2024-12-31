using ProjectGSMAUI.Api.Data;
using ProjectGSMAUI.Api.Data.Entities;
using ProjectGSMAUI.Api.Services;
using System.ComponentModel.Design;
using System.Security.Cryptography;

namespace ProjectGSMAUI.Api.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly ApplicationDbContext _context;
        public RefreshHandler(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create()) 
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var ExistToken = this._context.RefreshTokens.FirstOrDefault(item => item.UserID == username);
                if (ExistToken != null) 
                {
                    ExistToken.refreshtoken = refreshtoken;
                }
                else
                {
                    await this._context.RefreshTokens.AddAsync(new RefreshToken
                    {
                        UserID = username,
                        TokenID = new Random().Next().ToString(),
                        refreshtoken = refreshtoken
                    });
                }
                await this._context.SaveChangesAsync();
                return refreshtoken;
            }
        }
    }
}
