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

    public async Task<UsersEntity?> Delete(string email)
    {
        try
        {
            UsersEntity? user = await _context.users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

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

    public async Task<UsersEntity?> Get(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("Email is required");

            return await _context.users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
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

            UsersEntity? userFound = await _context.users
                .AsNoTracking().FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userFound is null)
                return null;

            _context.Entry(user).State = EntityState.Modified;
            await this._context.SaveChangesAsync(); 
            return userFound; 
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao atualizar usuário: {e.Message}", e);
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password are required");

            UsersEntity? userFound = await _context.users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userFound is null)
                return false;

            if (userFound.Password != password)
                return false;

            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Erro ao realizar login: {e.Message}", e);
        }
    }


}
