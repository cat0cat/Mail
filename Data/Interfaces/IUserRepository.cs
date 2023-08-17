using YOApi.Models;

namespace YOApi.Data.Interfaces;

public interface IUserRepository
{
    public Task<bool> CreateUser(UserRequest user, CancellationToken token);
    public Task<User?> GetUserByAddress(string address, CancellationToken token);
    public string HashPassword(UserRequest user); 
}