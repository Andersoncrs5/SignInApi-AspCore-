using Microsoft.EntityFrameworkCore;
using SignInApi.Context;
using SignInApi.Entities;
using SignInApi.SetRepositories.InterfacesRepositories;

namespace SignInApi.SetRepositories.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _context;

    public UsersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UsersEntity> Create(UsersEntity user)
    {
        try
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            await _context.users.AddAsync(user);
            return user;
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao criar usuário: {e.Message}", e);
        }
    }

    public async Task<UsersEntity?> Delete(ulong id)
    {
        try
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return null; 

            _context.users.Remove(user);
            return user; 
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao deletar usuário: {e.Message}", e);
        }
    }

    public async Task<UsersEntity?> Get(ulong id)
    {
        try
        {
            return await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao buscar usuário: {e.Message}", e);
        }
    }

    public async Task<UsersEntity?> Update(UsersEntity user)
    {
        try
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            UsersEntity? userFound = await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userFound is null)
                return null;

            _context.Entry(user).State = EntityState.Modified;
            //_context.Entry(userFound).CurrentValues.SetValues(user);
            await this._context.SaveChangesAsync(); 
            return userFound; 
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao atualizar usuário: {e.Message}", e);
        }
    }
}
