namespace InterfaceServer.Services
{
    public interface IRefreshHandler
    {
        Task<string> GenerateToken(string username);
    }
}
