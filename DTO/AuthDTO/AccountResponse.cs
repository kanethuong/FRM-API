namespace kroniiapi.DTO.AuthDTO
{
    public class AuthResponse
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string AvatarURL { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
    }
}