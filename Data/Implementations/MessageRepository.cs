using YOApi.Data.Interfaces;
using YOApi.Models;
using YOApi.Models.Messages;
using Microsoft.EntityFrameworkCore;

namespace YOApi.Data.Implementations;

public class MessageRepository : IMessageRepository
{      
    private readonly YOApiContext _context;

    public MessageRepository(YOApiContext context)
    {
        _context = context;
    }

    public async Task CreateMessage(MessageFromClient messageData, int userId, CancellationToken token)
    {
        UserMessage userSender = new UserMessage(userId, Role.Sender);
        var userMessages = new List<UserMessage>();
        userMessages.Add(userSender);

        var recipient = await _context.Users.SingleOrDefaultAsync(x => x.Address == messageData.Recipient, token);
        if (recipient == null)
            throw new RecepientNotFoundException(new List<string> {messageData.Recipient}); 
        UserMessage userRecipient = new UserMessage(recipient.Id, Role.Recipient);
        userMessages.Add(userRecipient);
        // foreach (var address in messageData.CopyTo)
        // {
        //     var recipientCopy = await _context.Users.SingleOrDefaultAsync(x => x.Address == address, token);
        //     if (recipientCopy == null)
        //         throw new RecepientNotFoundException();
        //     userMessages.Add(new UserMessage(recipientCopy.Id, Role.Copy));
        // }
        var userCopy = _context.Users.Where(x => messageData.CopyTo.Contains(x.Address));
        var id = userCopy.Select(x => x.Id);
            
        var userNotFound = messageData.CopyTo.ExceptBy(userCopy.Select(x => x.Address), y => y).ToList();
        if (!userNotFound.Any())
            throw new RecepientNotFoundException(userNotFound);

        userMessages.AddRange(id.Select(x => new UserMessage(x, Role.Copy)));
   
        var message = new Message(messageData, userMessages); 
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(token);
    }

    public async Task<MessageBody?> GetMessageById(int messageId, int userId, CancellationToken token)
    {
        var body = await _context.Messages.FirstOrDefaultAsync(x => x.Id == messageId && x.UserMessages.Any(um => um.UserId == userId), token);
        if(body is null)
            return null;
        return new MessageBody(){Body = body.Body};
    }

    public async Task<bool> MessageUpdateReadStatus(int messageId, CancellationToken token)
    {
        var message = await _context.Messages.FirstOrDefaultAsync(p => p.Id == messageId, token);
        if (message is null)
            return false;
        _context.UserMessages.Single(x => x.MessageId == messageId).IsRead = true;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> MessageUpdateArchiveStatus(List<int> messageId, int userId, CancellationToken token)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, token);
        if (user is null)
            return false;
        foreach (var message in await _context.Messages.Where(m => messageId.Contains(m.Id)).Include(x => x.UserMessages).ToListAsync(token))
        {
            foreach (var m in message.UserMessages.Where(um => um.UserId == userId))
            {
                m.IsArchive = true;
            }    
        }
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<MessagesBox> GetMessagesByUserId(int userId, MailBoxes selection, ArchiveBoxes location, CancellationToken token) 
    {
        MessagesBox answer = new MessagesBox();
        var user = await _context.Users.SingleAsync(x => x.Id == userId, token);
        if (user is null)
            throw new BadHttpRequestException("Invalid user id");

        IQueryable<UserMessage> um = _context.UserMessages
            .Include(m => m.Message)
            .ThenInclude(m => m.Users);
            um = um.Where(x => x.UserId == userId);
        if (selection == MailBoxes.Sender)
            um = um.Where(x => x.Role == Role.Sender);
        else if (selection == MailBoxes.Recipient)
            um = um.Where(x => x.Role == Role.Recipient || x.Role == Role.Copy);
        if (location == ArchiveBoxes.Archivated)
            um = um.Where(x => x.IsArchive);
        else if (location == ArchiveBoxes.NotArchivated)
            um = um.Where(x => !x.IsArchive);

        var head = um.Select(x => new MessageHead(x.Message, 
                                                  x.Message.UserMessages.Where(r => r.Role == Role.Copy)
                                                  .Select(um => um.User.Address).ToList()))
            .ToArray();
        answer.Messages = head;
        answer.Count = answer.Messages.Count();
        return answer;
    }

    public async Task<MessagesBox> Search(MailBoxes roleSelection, ArchiveBoxes archiveSelection, int userId, 
        string searchString, CancellationToken token) 
    {
        var user = await _context.Users.SingleAsync(x => x.Id == userId, token);
        IQueryable<UserMessage> um = _context.UserMessages
            .Include(m => m.Message)
            .ThenInclude(m => m.Users);
        if (roleSelection == MailBoxes.Sender)
            um = um.Where(x => x.Role == Role.Sender);
        else if (roleSelection == MailBoxes.Recipient)
            um = um.Where(x => x.Role == Role.Recipient || x.Role == Role.Copy);
        if (archiveSelection == ArchiveBoxes.Archivated)
            um = um.Where(x => x.IsArchive);
        else if (archiveSelection == ArchiveBoxes.NotArchivated)
            um = um.Where(x => !x.IsArchive);
        um = um.Where(x => x.UserId == userId && 
            (x.Message.Title.Contains(searchString) 
            || x.Message.Body!.Contains(searchString) 
            || x.Message.UserMessages.Any(um => um.User.Address.Contains(searchString))));
        var copyList =  um.Where(um => um.Role == Role.Copy && um.UserId == userId).Select(um => um.User.Address).ToList();
        var heads = um.Select(x => new MessageHead(x.Message, copyList)).ToArray();
        MessagesBox answer = new MessagesBox();
        answer.Messages = heads;
        answer.Count = answer.Messages.Count();
        return answer;
    }
}