using YOApi.Models.Messages;

namespace YOApi.Models;

public class UserMessage
{
    public int Id { get; set; }
    public int UserId { get; set; }
#nullable disable
    public User User { get; set; }
#nullable enable
    public int MessageId { get; set; }
#nullable disable
    public Message Message { get; set; }
#nullable enable
    public Role Role { get; set; }
    public bool IsArchive { get; set; }       
    public bool IsRead { get; set; }  

    public UserMessage (int userId, Role role){
        UserId = userId;
        Role = role;
    }
}