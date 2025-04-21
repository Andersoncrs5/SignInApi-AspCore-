using SignInApi.Entities;

namespace SignInApi.SetRepositories.InterfacesRepositories
{
    public interface IUsersRepository
    {
        Task<UsersEntity?> Get(string email);
        Task<UsersEntity?> Create(UsersEntity user);
        Task<UsersEntity?> Update(UsersEntity user);
        Task<UsersEntity?> Delete(string email);
        Task<bool> LoginAsync(string email, string password);
    }
}