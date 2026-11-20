using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }


        public int? FeatureId { get; set; }
        public bool IsRead { get; set; } = false;

        /// ///////////////////////////////
        public int? GlobalOrderId { get; set; }
    }
}
