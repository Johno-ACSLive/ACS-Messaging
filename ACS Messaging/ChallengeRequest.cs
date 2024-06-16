using System.Text.Json.Serialization;

namespace ACS.Messaging
{
    internal class ChallengeRequest
    {
        [JsonInclude]
        internal string ID { get; set; }
        [JsonInclude]
        internal ChallengeRequestType ChallengeType { get; set; }

        internal enum ChallengeRequestType
        {
            ChallengeRequested,
            ChallengeSuccessful,
        }
    }
}