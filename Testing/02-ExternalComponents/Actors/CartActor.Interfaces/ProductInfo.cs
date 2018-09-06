using System.Runtime.Serialization;

namespace CartActor.Interfaces
{
    [DataContract]
    public class ProductInfo
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public double Quantity { get; set; }
        [DataMember]
        public decimal UnitCost { get; set; }
    }
}