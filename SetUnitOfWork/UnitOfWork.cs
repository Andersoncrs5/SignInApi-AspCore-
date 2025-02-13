using SignInApi.Context;
using SignInApi.SetRepositories.InterfacesRepositories;
using SignInApi.SetRepositories.Repositories;

namespace SignInApi.SetUnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private IUsersRepository? _usersRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IUsersRepository UsersRepository
        => _usersRepository ??= new UsersRepository(_context);

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
