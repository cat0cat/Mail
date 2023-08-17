using Microsoft.AspNetCore.Mvc;
using YOApi.Data.Interfaces;
using YOApi.Models.Messages;
using Microsoft.AspNetCore.Authorization;
using YOApi.Data;

namespace YOApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageRepository _messageRepository;

    public MessageController(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
      
    [HttpGet("{messageId}")]
    public async Task<ActionResult> GetMessageById(int messageId, CancellationToken token)
    {
        var userId = CheckUserId();
        var message = await _messageRepository.GetMessageById(messageId, userId, token);
        if (message is null)
            return NotFound("Message not found");
        return Ok(message);
    }

    [HttpPost]
    public async Task<ActionResult> CreateMessage(MessageFromClient message, CancellationToken token)
    {
        var userId = CheckUserId();
        try
        {
            await _messageRepository.CreateMessage(message, userId, token);
        }
        catch (RecepientNotFoundException ex)
        {
            return BadRequest($"The recepient {string.Join(", ", ex.Value)} has not been found");
        }

        return Ok();
    }

    [HttpPut("updatereadstatus")]
    public async Task<ActionResult> MessageUpdateReadStatus(int messageId, CancellationToken token)
    {
        var check = await _messageRepository.MessageUpdateReadStatus(messageId, token);
        if (!check)
            return NotFound("Message not found");
        return Ok();
    }

    [HttpPut("archivestatus")]
    public async Task<ActionResult> MessageUpdateArchiveStatus(List<int> messageId, CancellationToken token)
    {
        var userId = CheckUserId();
        var check = await _messageRepository.MessageUpdateArchiveStatus(messageId, userId, token);
        if (!check)
            return NotFound("Some of the messages you selected may not have been moved to the target directory");           
        return Ok();

    }

    [HttpGet("all")]
    public async Task<ActionResult> GetAllMessages([FromQuery] string action, CancellationToken token)
    {
        var id = CheckUserId();
        MessagesBox messages;
        switch (action) {
            case "incoming":
                messages = await _messageRepository.GetMessagesByUserId(id, MailBoxes.Recipient, ArchiveBoxes.NotArchivated, token);
                break;
            case "sent":
                messages = await _messageRepository.GetMessagesByUserId(id, MailBoxes.Sender, ArchiveBoxes.NotArchivated, token);
                break;
            default:
                return BadRequest("Invalid action");
        }
        return Ok(messages);
    }

    [HttpGet("archive")]
    public async Task<ActionResult<MessagesBox>> GetAllArchive([FromQuery] string action, CancellationToken token)
    {
        var id = CheckUserId();
        MessagesBox messages;
        switch (action) {
            case "incoming":
                messages = await _messageRepository.GetMessagesByUserId(id, MailBoxes.Recipient, ArchiveBoxes.Archivated, token);
                break;
            case "sent":
                messages = await _messageRepository.GetMessagesByUserId(id, MailBoxes.Sender, ArchiveBoxes.Archivated, token);
                break;
            default:
                return BadRequest("Invalid action");
        }
        return Ok(messages);
    }

    [HttpGet("search")]
    public async Task<ActionResult<MessagesBox>> Search([FromQuery] MailBoxes mailSelection, 
        ArchiveBoxes archiveSelection, string searchString, CancellationToken token)
    { 
        var id = CheckUserId();
        var messages = await _messageRepository.Search(mailSelection, archiveSelection, id, searchString, token);
        return Ok(messages);
    }

    private int CheckUserId() {
        return int.Parse(HttpContext.User.Claims.First(c => c.Type == "Id").Value);
    }
}