namespace Hospital.Core.Models.Requests
{
    public record class RefreshTokenRequest(int UserId, string RefreshToken);
}
