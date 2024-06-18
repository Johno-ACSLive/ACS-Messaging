using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACS.Messaging
{
    internal static class Json
    {
        internal static async Task<object> DeserializeAsync(Stream Stream, object Object)
        {
            // Can't use generics so we need to set object type
            return await JsonSerializer.DeserializeAsync(Stream, Object.GetType());
        }

        internal static async Task SerializeAsync(Stream Stream, object value)
        {
            await JsonSerializer.SerializeAsync(Stream, value);
        }

        internal static async Task<object> DeserializeAsync(string value)
        {
            return await Task.Run(() =>
            {
                return JsonSerializer.Deserialize<object>(value);
            });
        }

        internal static async Task<string> SerializeAsync(object value)
        {
            return await Task.Run(() =>
            {
                return JsonSerializer.Serialize(value);
            });
        }

        internal static object Deserialize(string value)
        {
            return JsonSerializer.Deserialize<object>(value);
        }

        internal static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}