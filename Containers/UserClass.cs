using AutoMapper;
using InterfaceServer.Helpers;
using InterfaceServer.Modal;
using InterfaceServer.Repos;
using InterfaceServer.Services;
using Microsoft.EntityFrameworkCore;

namespace InterfaceServer.Containers
{
    public class UserClass : IUserService
    {
        private readonly FileTestContext _context;
        private readonly IMapper mapper;
        private readonly ILogger<UserClass> _logger;
        public UserClass(FileTestContext context, IMapper mapper, ILogger<UserClass> logger)
        {
            this._context = context;
            this.mapper = mapper;
            this._logger = logger;
        }

        public async Task<APIResponse> Create(UserModal data)
        {
            APIResponse response = new APIResponse();
            try
            {
                this._logger.LogInformation("Create Begins");
                if (_context.Users.FirstOrDefault(u=>u.Login == data.Login) == null)
                {

                    User user = this.mapper.Map<UserModal, User>(data);
                    user.IdUser = Guid.NewGuid().ToString();
                    user.RoleIdRole = 2;

                    Playlist playlist = new Playlist()
                    {
                        IdPlaylist = Guid.NewGuid().ToString(),
                        Name = "Избранное",
                        UserIdUser = user.IdUser,
                        UserIdUserNavigation = user
                    };

                    user.Playlists = new List<Playlist>()
                    {
                        playlist
                    };

                    await this._context.AddAsync<Playlist>(playlist);
                    await this._context.AddAsync<User>(user);
                    await this._context.SaveChangesAsync();

                    response.ResponseCode = 201;
                    response.Result = user.IdUser;
                }
                else
                {
                    response.ResponseCode = 403;
                    response.Result = "Пользователь с таким именем уже зарегистрирован";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<List<UserModal>> GetAll()
        {
            List<UserModal> result = new List<UserModal>();
            var _data = await _context.Users.ToListAsync();
            if (_data != null)
            {
                result = this.mapper.Map<List<User>, List<UserModal>>(_data);
            }
            return result;
        }

        public async Task<UserModal> GetById(string id)
        {
            UserModal result = new UserModal();
            var _data = await _context.Users.FindAsync();
            if (_data != null)
            {
                result = this.mapper.Map<User, UserModal>(_data);
            }
            return result;
        }

        public async Task<APIResponse> Remove(string id)
        {
            var user = await _context.Users.FindAsync(id);
            var response = new APIResponse();

            try
            {
                if (user != null)
                {
                    _context.Remove<User>(user);

                    await _context.SaveChangesAsync();

                    response.ResponseCode = 201;
                    response.Result = user.IdUser;
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        public async Task<APIResponse> Update(UserModal data, string id)
        {
            APIResponse response = new APIResponse();
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (_context.Users.FirstOrDefault(u => u.Login == data.Login) == null && user != null)
                {

                    user.Login = data.Login;
                    user.Password = data.Password;

                    await this._context.SaveChangesAsync();

                    response.ResponseCode = 201;
                    response.Result = user.IdUser;
                }
                else
                {
                    response.ResponseCode = 403;
                    response.Result = "Пользователь с таким именем уже зарегистрирован";
                }

            }
            catch (Exception ex)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
