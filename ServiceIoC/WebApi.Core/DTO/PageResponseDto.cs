using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Core.DTO
{
    public class PageResponseDto<TEntity>
    {
        [JsonProperty("records")]
        public IEnumerable<TEntity> Records { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalItems")]
        public int TotalItems { get; set; }
        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }
        [JsonProperty("firstPage")]
        public string FirstPageUrl { get; set; }
        [JsonProperty("lastPage")]
        public string LastPageUrl { get; set; }
        [JsonProperty("nextPage")]
        public string NextPageUrl { get; set; }
        [JsonProperty("prevPage")]
        public string PreviousPageUrl { get; set; }
    }
}
