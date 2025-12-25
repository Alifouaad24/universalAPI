namespace Universal_server.Models.Helper_models
{
    public class ServiceDto
    {

        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public List<int>? BusinessesId { get; set; }
        public List<int>? ActivitiesId { get; set; }
        public string? Service_icon { get; set; }
    }
}
