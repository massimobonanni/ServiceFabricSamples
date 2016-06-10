using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using WebApi.Core.DTO;

namespace WebApi.Core.Responses
{
    public abstract class ResponseBase
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; } = true;

        [JsonProperty("errors")]
        public IEnumerable<ErrorDto> Errors { get; set; }
    }

    public abstract class ResponseBase<TResponseDto> : ResponseBase
    {
        [JsonProperty("data")]
        public TResponseDto Data { get; set; }
    }

}
