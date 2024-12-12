namespace ProjectToDo.Services.Dto
{
    public class LoginDto
    {
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
    }
}
