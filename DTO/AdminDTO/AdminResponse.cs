namespace kroniiapi.DTO.AdminDTO
{
    public class AdminResponse
    {
        public int AdminId { get; set; }
        public string Fullname { get; set; }
        public string AvatarURL { get; set; }
        public string Email { get; set; }
        public decimal Wage { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}