using YOApi.Models.Messages;

namespace YOApi.Models;

public class User
{
    public int Id { get; set; }
#nullable disable
    public string Address { get; set; }
    public string Password { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<UserMessage> UserMessages { get; set; }
#nullable enable
}