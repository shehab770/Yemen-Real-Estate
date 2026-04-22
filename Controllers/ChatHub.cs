using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using webProgramming.Models;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;
    protected readonly AppDbContext _context;
    
    public ChatHub(AppDbContext Context, ILogger<ChatHub> logger) {
        _context = Context;
        _logger = logger;
    }


   public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        _logger.LogInformation($"User connected: {userId}, ConnectionId: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public async Task UpdateUnreadCount(string chatId)
    {
        Console.WriteLine("---------------------------------" );
        Console.WriteLine("UpdateUnreadCount: " + chatId);
        Console.WriteLine("---------------------------------" );
        var userId = Context.UserIdentifier;
        Console.WriteLine("---------------------------------" );
        Console.WriteLine("UpdateUnreadCount: " + userId);
        Console.WriteLine("---------------------------------" );
        var unreadCount = await CalculateUnreadCount(chatId, userId);
        Console.WriteLine("---------------------------------" );
        Console.WriteLine("UpdateUnreadCount: " + unreadCount);
        Console.WriteLine("---------------------------------" );
        await Clients.Caller.SendAsync("UpdateUnreadCount", chatId, unreadCount);
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        _logger.LogInformation($"User {Context.UserIdentifier} joined chat {chatId}");
    }

    public async Task MarkMessagesAsRead(string chatId)
    {
        var userId = Context.UserIdentifier;

        var messages = await _context.Messages.Where(m => m.ChatId == chatId && m.SenderId != userId && m.IsRead == 0).ToListAsync();
        foreach (var message in messages)
        {
            message.IsRead = 1;
        }
        await _context.SaveChangesAsync();

        var unreadCount = await CalculateUnreadCount(chatId, userId);
        await Clients.Caller.SendAsync("UpdateUnreadCount", chatId, unreadCount);
    }

    private async Task<int> CalculateUnreadCount(string chatId, string userId)
    {
        // TODO: Query DB for count of unread messages
        var unreadCount = await _context.Messages.Where(m => m.ChatId == chatId && m.SenderId != userId && m.IsRead == 0).CountAsync();
        return unreadCount;
    }
}
