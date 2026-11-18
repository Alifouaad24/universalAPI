using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class BusinessRelation
    {
        [Key]
        public int BusinessRelationId { get; set; }

        // البزنس الذي يقدم الخدمة
        [JsonIgnore]
        public Business? ProviderBusiness { get; set; }
        [ForeignKey(nameof(ProviderBusinessId))]
        public int ProviderBusinessId { get; set; }

        /// ////////////////////////////////////////////////////////////
        /// 
        [JsonIgnore]
        public Business? ConsumerBusiness { get; set; }
        // البزنس الذي يستفيد من الخدمة
        [ForeignKey(nameof(ConsumerBusinessId))]
        public int? ConsumerBusinessId { get; set; }

        /// ////////////////////////////////////////////////////////////
        
        public GlobalCustomer? GlobalCustomer { get; set; }
        // العميل الذي يستفيد من الخدمة
        [ForeignKey(nameof(GlobalCustomer))]
        public int? GlobalCustomerId { get; set; }
        
        /// ///////////////////////////////////////////////////////

    }
}
