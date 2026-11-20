using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models.Helper_models
{
    public class FeatureDto
    {
        public string? Body { get; set; }
        public string? Status { get; set; }
        public string? Comment { get; set; }
        public int? GlobalSystemId { get; set; }
        public int? BusinessId { get; set; }
        public int? ServiceId { get; set; }
    }


    public class CommentBody
    {
        public string comment { get; set; }
    }
}
