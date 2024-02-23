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
        public Uri MaxRetriesForFetchingEssayContent { get; set; }
    }
}
