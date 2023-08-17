using YOApi.Data.Interfaces;
using YOApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace YOApi.Data.Implementations;

class UserRepository : IUserRepository
{
    private readonly YOApiContext _context;
    public UserRepository(YOApiContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateUser(UserRequest user, CancellationToken token)
    {        
        user.Password = HashPassword(user);
        User _user = new User()
        {
            Address = user.Address,
            Password = user.Password
        };
        try
        {
            var a = await _context.Users.AddAsync(_user, token);
            await _context.SaveChangesAsync(token);
        }
        catch (DbUpdateException)
        {
            return false;
        }
        return true;
    }

    public async Task<User?> GetUserByAddress(string address, CancellationToken token)
    {
        return await _context.Users.FirstOrDefaultAsync<User>(a => a.Address == address, token);
    }

    public string HashPassword(UserRequest user) 
    { 
        string mySecret = "4u7x!A%D*G-KaPdS"; 
        var key = Encoding.ASCII.GetBytes(mySecret); 
        return Convert.ToBase64String(KeyDerivation.Pbkdf2( 
            password: user.Password, 
            salt: key, 
            prf: KeyDerivationPrf.HMACSHA256, 
            iterationCount: 100000, 
            numBytesRequested: 256 / 8)); 
    }
}