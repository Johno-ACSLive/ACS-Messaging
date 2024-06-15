using System.Text.Json.Serialization;

namespace ACS.Messaging
{
    internal class ChallengeRequest
    {
        [JsonInclude]
        internal string ID { get; set; }
    }
}