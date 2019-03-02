using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Infrastructure.Localization
{
    public class Resource
    {
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("resources")]
        public Dictionary<string, string> Pairs { get; set; }
    }
}
