using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebApi.Core.Requests
{
    public abstract class PagedRequestBase : RequestBase
    {
        [JsonProperty("start")]
        public int Start { get; set; } = 0;

        [JsonProperty("numItems")]
        public int NumItems { get; set; } = 10;

        private string _orderBy;
        [JsonProperty("orderBy")]
        public string OrderBy
        {
            get
            {
                return _orderBy;

            }
            set
            {
                _orderBy = value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    OrderingFields = null;
                }
                else
                {
                    var orderingFields = new List<OrderingField>();
                    var orderSplit = value.Split(',');
                    foreach (var field in orderSplit)
                    {
                        var fieldSplit = field.Split(' ');
                        OrderingField orderingField = null;
                        if (fieldSplit.Count() > 1)
                            orderingField = new OrderingField(
                                fieldSplit[0],
                                String.Compare(fieldSplit[1], "asc", StringComparison.OrdinalIgnoreCase) == 0 ? OrderDirection.Ascending : OrderDirection.Descending);
                        else
                            orderingField = new OrderingField(
                                fieldSplit[0]);
                        orderingFields.Add(orderingField);
                    }
                    OrderingFields = orderingFields;
                }
            }
        }

        public IEnumerable<OrderingField> OrderingFields { get; private set; }
    }
}
