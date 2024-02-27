using InterfaceServer.Repos;
using InterfaceServer.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace InterfaceServer.Containers
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly FileTestContext _context;
        public RefreshHandler(FileTestContext context)
        {
            this._context = context;
        }
        public async Task<string> GenerateToken(string userId)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);

                var existToken = await this._context.RefreshTokens.FirstOrDefaultAsync(item => item.UserId == userId);

                if (existToken != null)
                {
                    existToken.RefreshToken1 = refreshToken;
                }
                else
                {
                    await this._context.RefreshTokens.AddAsync(new RefreshToken()
                    {
                        RefreshToken1 = refreshToken,
                        UserId = userId,
                        TokenId = Guid.NewGuid().ToString()
                    });
                }
                await this._context.SaveChangesAsync();
                return refreshToken;
            }
        }
    }
}
