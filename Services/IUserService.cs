using InterfaceServer.Helpers;
using InterfaceServer.Modal;
using InterfaceServer.Repos;

namespace InterfaceServer.Services
{
    public interface IUserService
    {
        Task<List<UserModal>> GetAll();

        Task<UserModal> GetById(string id);

        Task<APIResponse> Remove(string id);

        Task<APIResponse> Create(UserModal data);

        Task<APIResponse> Update(UserModal data, string id);
    }
}
