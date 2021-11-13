namespace kroniiapi.DTO.ApplicationDTO
{
    public class TraineeApplicationResponse
    {
        public string Description { get; set; }
        public string ApplicationURL { get; set; }
        public string Type { get; set; }
        public bool? IsAccepted { get; set; }
    }
}