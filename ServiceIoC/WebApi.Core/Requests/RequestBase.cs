using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Core.Requests
{
    public abstract class RequestBase
    {
        [JsonProperty("lang")]
        public string Language { get; set; } = "it-IT";

    }

    public abstract class RequestBase<TPayload> : RequestBase
    {
        [JsonProperty("data")]
        public TPayload Data { get; set; }

    }
}
