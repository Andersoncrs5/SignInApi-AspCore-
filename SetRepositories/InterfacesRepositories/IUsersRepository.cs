using SignInApi.Entities;

namespace SignInApi.SetRepositories.InterfacesRepositories
{
    public interface IUsersRepository
    {
        Task<UsersEntity> Get(ulong id);
        Task<UsersEntity> Create(UsersEntity user);
        Task<UsersEntity> Update(UsersEntity user);
        Task<UsersEntity> Delete(ulong id);
    }
}