using Microsoft.AspNetCore.SignalR;

namespace webProgramming.Controllers
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            var userId = httpContext?.Request.Query["userId"].ToString();

            return !string.IsNullOrEmpty(userId) ? userId : "anonymous";
        }
    }
}
