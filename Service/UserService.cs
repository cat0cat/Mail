using YOApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YOApi.Options;
using Microsoft.Extensions.Options;

namespace YOApi.Sevice;

public interface IUserService
{
    public string GenToken(User user);
}

public class UserService : IUserService
{
    private const int TimeHours = 5;
    private readonly IOptions<TokenOptions> _tokenOptions;

    public UserService(IOptions<TokenOptions> tokenOptions)
    {
        _tokenOptions = tokenOptions;
    }
        
    public string GenToken(User user)
    {
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {    
        //generate token that is valid for 5 hours
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenOptions.Value.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            { 
                new Claim("Id", user.Id.ToString()),
                new Claim("Address", user.Address)
                }),
            Expires = DateTime.UtcNow.AddHours(TimeHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}