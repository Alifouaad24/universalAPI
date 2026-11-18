using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class GlobalOrderDto
    {
        public int? GlobalCustomerId { get; set; }
        public int Business_id { get; set; }
        public DateOnly? Schedule_dt { get; set; }
        public string? Schedule_time { get; set; }
        public string? Notes { get; set; }
        public string? Domain { get; set; }
        public string? Name { get; set; }
        public int? CarInfoTblId { get; set; } // null
        public int? Platform_id { get; set; }
        public int? ShippingTypeId { get; set; }
        public int? Service_id { get; set; }
        public string? AssignerId { get; set; }
        public string? AssigneeId { get; set; }
        public int? AssignerTypeId { get; set; }
        public int? AssigneeTypeId { get; set; }
        public DateOnly? DeadLine { get; set; }
        public DateOnly? StartProgressDate { get; set; }
        public DateOnly? EndTaskDate { get; set; }

        public int? OrderStatusId { get; set; }
        public int? UnitId { get; set; }
        public int? UnitValue { get; set; }
        public string? Cart_value { get; set; }
        public string? Port { get; set; }
        public string? ImagePath { get; set; }
        public string? CreatedBy { get; set; }
        public string? Modified_by { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public List<PackageDto>? packages { get; set; }
    }
}

public class PackageDto
{
    public int unitId { get; set; }
    public decimal? value { get; set; }
    public string? imageUrl { get; set; }
}
