using System.Runtime.Serialization;

namespace CartActor.Interfaces
{
    [DataContract]
    internal class ProductData
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public double Quantity { get; set; }
        [DataMember]
        public decimal UnitCost { get; set; }
    }
}