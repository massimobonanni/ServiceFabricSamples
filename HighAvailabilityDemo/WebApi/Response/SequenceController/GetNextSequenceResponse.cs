﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Response.SequenceController
{
    public class GetNextSequenceResponse
    {
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("nodeInfo")]
        public string NodeInfo { get; set; }
        [JsonProperty("actorId")]
        public string ActorId { get; set; }
    }
}