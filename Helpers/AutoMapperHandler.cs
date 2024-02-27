using AutoMapper;
using InterfaceServer.Modal;
using InterfaceServer.Repos;

namespace InterfaceServer.Helpers
{
    public class AutoMapperHandler:Profile
    {
        public AutoMapperHandler() {

            CreateMap<User, UserModal>().
                ForMember(item=>item.Role, opt=>opt.MapFrom(item=>item.RoleIdRole==1?"Admin":"User")).
                ReverseMap();
            
        
        }
    }
}
