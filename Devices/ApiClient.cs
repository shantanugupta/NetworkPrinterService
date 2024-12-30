using System.Net.Http.Json;

public static class ApiClient
{
    private static SemaphoreSlim Throttler;

    static ApiClient()
    {
        Throttler = new SemaphoreSlim(1, 1);
    }

    public static async Task<T> GetRequestAsync<T>(this HttpClient Proxy, string path)
    {
        T result = default;

        HttpResponseMessage response = await Proxy.GetAsync(path);
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<T>();
        }

        return result;
    }

    public static T GetRequest<T>(this HttpClient Proxy, string path)
    {
        T result = default;
        var response = Proxy.GetAsync(path).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseContent = response.Content;

            // by calling .Result you are synchronously reading the result
            result = responseContent.ReadFromJsonAsync<T>().Result;
        }
        return result;
    }

    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(HttpClient Proxy, string requestUri, T value)
    {
        return await Proxy.PostAsJsonAsync(requestUri, value);
    }

    public static async Task<T> PostRequestAsync<T>(this HttpClient Proxy, string path, object request)
    {
        T result = default;
        HttpResponseMessage response = await Proxy.PostAsJsonAsync(path, request);
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<T>();
        }
        return result;
    }

    public static async Task<T> PostFileAsync<T>(this HttpClient Proxy, string path, HttpContent request)
    {
        T result = default;
        HttpResponseMessage response = await Proxy.PostAsync(path, request);
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<T>();
        }
        return result;
    }

    public static async Task<T> PostRequestAsyncThrottling<T>(this HttpClient Proxy, string path, object request)
    {

        bool entered = await Throttler.WaitAsync(TimeSpan.Zero);
        T result = default;
        HttpResponseMessage response = await Proxy.PostAsJsonAsync(path, request);
        if (response.IsSuccessStatusCode)
        {
            result = await response.Content.ReadFromJsonAsync<T>();
            Throttler.Release();
        }
        else
        {
            Throttler.Release();
        }
        return result;
    }
}
