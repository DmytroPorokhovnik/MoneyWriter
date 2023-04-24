using System.Net.Http.Json;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static object GetJsonMessage(this string message)
        {
            var jsonContent = JsonContent.Create(new {message = message});
            return jsonContent.Value ?? new object();
        }
    }
}
