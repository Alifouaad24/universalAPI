namespace Universal_server.Models
{
    public class RegisterUserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<int>? BusinessIds { get; set; }
    }
}
