using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class ActivityDto
    {

        public string Description { get; set; }
        public int Business_id { get; set; }
    }
}
