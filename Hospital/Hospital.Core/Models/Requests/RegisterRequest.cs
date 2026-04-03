namespace Hospital.Core.Models.Requests
{
    public record class RegisterRequest(string UserName, string Email, string Password);
}
