using FirebaseAdmin.Messaging;

namespace Universal_server.firebase_options
{
    public interface IFirebaseNotificationService
    {
        Task SendAsync(string token, string title, string body);
    }

    public class FirebaseNotificationService
    : IFirebaseNotificationService
    {
        public async Task SendAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,

                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },

                Data = new Dictionary<string, string>
                {
                    { "type", "task" }
                }
            };

            await FirebaseMessaging.DefaultInstance
                .SendAsync(message);
        }
    }
}
