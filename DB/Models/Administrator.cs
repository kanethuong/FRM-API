namespace kroniiapi.DB.Models
{
    public class Administrator
    {
        public int AdministratorId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // One-Many role
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}