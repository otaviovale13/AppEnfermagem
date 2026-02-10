namespace AppEnfermagem.Models
{
    public class LoginRequestDto
    {
        public string Username { get; set; } // <--- Adicione isto!
        public string Password { get; set; }
    }
}