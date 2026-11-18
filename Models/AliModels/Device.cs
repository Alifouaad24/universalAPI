using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.AliModels
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAdmin { get; set; }
        public DateOnly RequestData { get; set; }
    }
}
