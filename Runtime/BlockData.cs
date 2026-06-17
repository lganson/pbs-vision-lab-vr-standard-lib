using Newtonsoft.Json;

namespace Standard_Library
{
    public class BlockData : DataSerializer
    {
        [JsonProperty(Order = -4)]
        public string blockName;
        [JsonProperty(Order = -3)]
        public float timeStart;
        [JsonProperty(Order = -2)]
        public float timeEnd;
    }
}