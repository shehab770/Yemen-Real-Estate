using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public class ChatController : FunctionalController
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(AppDbContext context, IHubContext<ChatHub> hubContext) : base(context)
        {
            _hubContext = hubContext;
        }

        public IActionResult CreateChat(IFormCollection form)
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }
            
            var sellerId = form["SellerId"].ToString();
            var productId = form["ProductId"].ToString();

            var seller = _appDbContext.Users.FirstOrDefault(s => s.UserId == sellerId);
            var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);

            var viewModel = new ViewModels
            {
                User = user,
                Seller = seller,
                Product = product
            };

            return View(viewModel);

        }

        public IActionResult Messages(string id)   
       {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
            var chats = _appDbContext.Chats.
            Where(c => 
                user.Role == UserRole.Buyer ? c.BuyerId == user.UserId : c.SellerId == user.UserId
             ).OrderByDescending(c=>c.CreatedAt).ToList();
            var productIds = chats.Select(c => c.ProductId).ToList();
            var products = _appDbContext.Products.Where(p => productIds.Contains(p.ProductId)).OrderByDescending(p=>p.CreatedAt).ToList();
            var messages = _appDbContext.Messages.ToList();
            var viewModel = new ViewModels
            {
                User = user,
                Chats = chats,
                Products = products,
                Messages = messages
            };
            
            ViewBag.ChatId = id;
            
            return View(viewModel);
       }
    
        public IActionResult StoreChat(IFormCollection form)
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var sellerId = form["SellerId"].ToString();
            var productId = form["ProductId"].ToString();
            var buyerId = form["BuyerId"].ToString();
            var content = form["Content"].ToString();

            if(string.IsNullOrEmpty(sellerId) || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(buyerId) || string.IsNullOrEmpty(content))
            {
                TempData["ErrorMessage"] = "Please fill all the fields";
                return RedirectToAction("CreateChat");
            }

            var chat = new Chat
            {
                ChatId = Guid.NewGuid().ToString(),
                SellerId = sellerId,
                BuyerId = buyerId,
                ProductId = productId,
                CreatedAt = DateTime.Now
            };
             _appDbContext.Chats.Add(chat);
            _appDbContext.SaveChanges();


            var message = new Message
            {
                ChatId = chat.ChatId,
                Content = content,
                CreatedAt = DateTime.Now,
                SenderId = user.UserId,
                ReceiverId = sellerId
            };
            _appDbContext.Messages.Add(message);
            _appDbContext.SaveChanges();

           TempData["SuccessMessage"] = "Message sent successfully";
            return RedirectToAction("Messages", "Chat");
            
            
            
        }
    
        public IActionResult GetMessage(string chatId)
        {
            var chat = _appDbContext.Chats.FirstOrDefault(c => c.ChatId == chatId);
            if (chat == null) return Json(new { success = false });

            var currentUser = GetCurrentloggedUser();
            if(currentUser == null) return Json(new { success = false });

            var chatPartner = _appDbContext.Users.Where(
                u => currentUser.Role == UserRole.Buyer ? u.UserId == chat.SellerId : u.UserId == chat.BuyerId
                ).Select(u => new {
                    userId = u.UserId,
                    name = u.Name,
                    image = u.Image,
                    role = u.Role
                }).FirstOrDefault();

            
            var messages = _appDbContext.Messages.Where(m => m.ChatId == chatId).OrderBy(m => m.CreatedAt)
            .Select(m => new 
            {
                isCurrentUser = m.SenderId == currentUser.UserId,
                content = m.Content,
                timestamp = m.CreatedAt.ToString("MMM dd, HH:mm"),
            }).ToList();

            var html = "";
            foreach (var msg in messages)
            {
                html += $@"<div class='message {(msg.isCurrentUser ? "sent" : "received")}'>
                    <div class='message-content'>
                        <div>{msg.content}</div>
                        <small class='text-muted'>{msg.timestamp}</small>
                    </div>
                </div>";
            }

            return Json(new { success = true, messages = html, chatPartner = chatPartner});
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string chatId, string content, string receiverId)
        {
            try
            {
                var user = GetCurrentloggedUser();
                if (user == null) return Json(new { success = false, message = "User not logged in" });

                var chat = _appDbContext.Chats.FirstOrDefault(c => c.ChatId == chatId);
                if (chat == null) return Json(new { success = false, message = "Chat not found" });

                var message = new Message
                {
                    ChatId = chatId,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    SenderId = user.UserId,
                    ReceiverId = receiverId,
                    IsRead = 0
                };

                _appDbContext.Messages.Add(message);
                await _appDbContext.SaveChangesAsync();
                var timestamp = message.CreatedAt.ToString("HH:mm");

                var unreadCount = await _appDbContext.Messages.Where(m => m.ChatId == chatId && m.SenderId != user.UserId && m.IsRead == 0).CountAsync();
             await _hubContext.Clients.Group(chatId).SendAsync("ReceiveMessage", chatId, user.UserId, content, timestamp);


                    
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    
    }
}