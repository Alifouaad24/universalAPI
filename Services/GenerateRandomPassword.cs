namespace Universal_server.Services
{
    public class GenerateRandomPassword
    {
        public static Task<string> GeneratePassword()
        {
            var random = new Random();
            return Task.FromResult(random.Next(0, 100000).ToString("D5"));
        }
    }
}
