using SignInApi.SetRepositories.InterfacesRepositories;

namespace SignInApi.SetUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUsersRepository UsersRepository { get; }

        Task Commit(); 
    }
}
