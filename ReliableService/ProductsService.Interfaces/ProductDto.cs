using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProductsService.Interfaces
{
    [DataContract]
    public class ProductDto
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal UnitCost { get; set; }

        [DataMember]
        public int StoreUnit { get; set; }

        [DataMember]
        public ProductCategory Category { get; set; }

        public override string ToString()
        {
            return $"{Code} - {Description}";
        }
    }
}
