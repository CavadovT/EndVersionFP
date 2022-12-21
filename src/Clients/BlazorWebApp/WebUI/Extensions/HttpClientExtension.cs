using System.Net.Http.Json;

namespace WebUI.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<TResult?> PostGetResponseAsync<TResult, TValue>(this HttpClient Client, String Url, TValue Value)
        {
            var httpResponse = await Client.PostAsJsonAsync(Url, Value);

            return httpResponse.IsSuccessStatusCode ? await httpResponse.Content.ReadFromJsonAsync<TResult>() : default;
        }

        public static async Task PostAsync<TValue>(this HttpClient Client, String Url, TValue Value)
        {
            await Client.PostAsJsonAsync(Url, Value);
        }

        public static async Task<T?> GetResponseAsync<T>(this HttpClient Client, String Url)
        {
            return await Client.GetFromJsonAsync<T>(Url);
        }
    }
}