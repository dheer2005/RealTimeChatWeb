using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Context;
using RealtimeChat.Models;

namespace RealtimeChat.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    //public class ChatHubController : Controller
    //{
    //    private readonly ChatDbContext _context;
    //    public ChatHubController(ChatDbContext context)
    //    {
    //        _context = context;
    //    }


    //    [HttpPost]
    //    [Route("SendChatData")]
    //    public async Task<IActionResult> SendChatData([FromBody] Messages msg)
    //    {
    //        msg.Created = DateTime.UtcNow; 
    //         _context.Messages.Add(msg);
    //        await _context.SaveChangesAsync();
    //        return Ok(msg);
    //    }
    //    [HttpGet]
    //    [Route("GetUserChat/{from}/{to}")]
    //    public async Task<IActionResult> GetUserChat(string from , string to)
    //    {
    //     var list =   _context.Messages.Where(e => e.FromUser == from && e.ToUser == to).OrderBy(e => e.Created).ToList();

    //        if(from != to)
    //        {
    //            var list1 = _context.Messages.Where(e => e.FromUser == to && e.ToUser == from).OrderBy(e => e.Created).ToList();
    //            list.AddRange(list1);
    //        }
    //     //var list1 = _context.Messages.Where(e => e.FromUser == to && e.ToUser == from).OrderBy(e => e.Created).ToList();
    //     //   list.AddRange(list1);
    //        return Ok(list);
    //    }


    //}



    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChatController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet("{From}/{user}")]
        public async Task<IActionResult> GetMessages(string From, string user)
        {

            var list = await _context.Messages.Where(e => e.FromUser == From && e.UserTo == user).OrderBy(e => e.Created).ToListAsync();

            if (From != user)
            {
                var list1 = await _context.Messages.Where(e => e.FromUser == user && e.UserTo == From).OrderBy(e => e.Created).ToListAsync();
                list.AddRange(list1);
            }
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessage([FromBody] Messages message)
        {
            message.Created = DateTime.UtcNow.AddHours(5.5);
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Ok(message);
        }

        [HttpPost("groupChat")]
        public async Task<IActionResult> SaveGroupChats([FromBody] Group_chats group)
        {
            group.Created = DateTime.UtcNow.AddHours(5.5);
            _context.GroupChats.Add(group);
            await _context.SaveChangesAsync();
            return Ok(group);
        }


        [HttpGet("getGroupMessages/{groupName}")]
        public async Task<IActionResult> GetGroupMessages(string groupName)
        {
            var messages = await _context.GroupChats
                .Where(g => g.GroupName == groupName)
                .OrderBy(g => g.Created)
                .ToListAsync();

            return Ok(messages);
        }

    }

}