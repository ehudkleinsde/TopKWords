using Newtonsoft.Json;

namespace TopKWordsConfigProvider
{
    public class TopKWordsConfig
    {
        /// <summary>
        /// The number of top words to find.
        /// </summary>
        [JsonProperty("TopKWords")]
        public int TopKWords { get; set; }

        [JsonProperty("LogFilePath")]
        public string LogFilePath { get; set; }
    }
}
