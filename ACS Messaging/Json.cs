using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACS.Messaging
{
    public static class Json
    {
        public static async Task<object> DeserializeAsync(Stream Stream)
        {
            return await JsonSerializer.DeserializeAsync<object>(Stream);
        }

        public static async Task SerializeAsync(Stream Stream, object value)
        {
            await JsonSerializer.SerializeAsync(Stream, value);
        }

        public static async Task<object> DeserializeAsync(string value)
        {
            return await Task.Run(() =>
            {
                return JsonSerializer.Deserialize<object>(value);
            });
        }

        public static async Task<string> SerializeAsync(object value)
        {
            return await Task.Run(() =>
            {
                return JsonSerializer.Serialize(value);
            });
        }

        public static object Deserialize(string value)
        {
            return JsonSerializer.Deserialize<object>(value);
        }

        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value);
        }
    }
}