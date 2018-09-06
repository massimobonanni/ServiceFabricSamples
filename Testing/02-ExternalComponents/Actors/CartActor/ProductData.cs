using System.Runtime.Serialization;

namespace CartActor
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