using System.Text.Json.Serialization;

namespace ACS.Messaging
{
    internal class ChallengeResponse
    {
        [JsonInclude]
        internal string ID { get; set; }
        [JsonInclude]
        internal string Challenge { get; set; }
    }
}