using Newtonsoft.Json;

namespace TopKWordsConfigProvider
{
    public class TopKWordsConfig
    {
        /// <summary>
        /// The number of top words to find.
        /// </summary>
        [JsonProperty("TopKWords", Required = Required.Always)]
        public int TopKWords { get; set; }

        /// <summary>
        /// Path to local log file.
        /// </summary>
        [JsonProperty("LogFilePath", Required = Required.Always)]
        public string LogFilePath { get; set; }

        /// <summary>
        /// Absolute Uri to essays list.
        /// </summary>
        [JsonProperty("EssaysListAbsoluteUri", Required = Required.Always)]
        public Uri EssaysListAbsoluteUri { get; set; }

        /// <summary>
        /// Max retries for fetching essay's content.
        /// </summary>
        [JsonProperty("MaxRetriesForFetchingEssayContent", Required = Required.Always)]
        public int MaxRetriesForFetchingEssayContent { get; set; }

        /// <summary>
        /// HttpClient pool capacity.
        /// </summary>
        [JsonProperty("HttpClientPoolCapacity", Required = Required.Always)]
        public int HttpClientPoolCapacity { get; set; }

        /// <summary>
        /// Max requests per minute.
        /// </summary>
        [JsonProperty("MaxRequestsPerMinute", Required = Required.Always)]
        public int MaxRequestsPerMinute { get; set; }

        /// <summary>
        /// Words bank Uri.
        /// </summary>
        [JsonProperty("WordsBankUri", Required = Required.Always)]
        public Uri WordsBankUri { get; set; }
    }
}
