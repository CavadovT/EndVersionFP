using System.Net.Http.Json;

namespace Web.ApiGateway.Extensions
{
    public static class HttpClientExtension
    {
        public async static Task<TResult?> PostGetResponseAsync<TResult, TValue>(this HttpClient Client, String Url, TValue Value)
        {
            var httpResponse = await Client.PostAsJsonAsync(Url, Value);

            return httpResponse.IsSuccessStatusCode ? await httpResponse.Content.ReadFromJsonAsync<TResult>() : default;
        }

        public async static Task PostAsync<TValue>(this HttpClient Client, String Url, TValue Value)
        {
            await Client.PostAsJsonAsync(Url, Value);
        }
        public async static Task<T?> GetResponseAsync<T>(this HttpClient Client, String Url)
        {
            return await Client.GetFromJsonAsync<T>(Url);
        }
    }
}
