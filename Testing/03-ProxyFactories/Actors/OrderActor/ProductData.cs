using System;
using System.Runtime.Serialization;
using OrderActor.Interfaces;

namespace OrderActor
{
    [DataContract]
    internal class ProductData
    {
        public ProductData()
        {
            
        }

        public ProductData(ProductInfo product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            this.Id = product.Id;
            this.Quantity = product.Quantity;
            this.UnitCost = product.UnitCost;
        }

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public double Quantity { get; set; }
        [DataMember]
        public decimal UnitCost { get; set; }
    }
}