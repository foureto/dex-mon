using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Flour.Commons;
using Flour.Commons.Models;
using Microsoft.Extensions.Logging;

namespace dex.monitor.Business.Helpers;

public class BasicHttpClient
{
    private readonly ILogger _logger;

    protected BasicHttpClient(HttpClient client, ILogger logger)
    {
        Client = client;
        _logger = logger;
    }

    public HttpClient Client { get; }

    protected Task<Result<T>> Get<T>(
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Get, null, uri, headers, token);
    }

    protected Task<Result<T>> GetForm<T>(
        string uri,
        Dictionary<string, object> formData = null,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return CallForm<T>(HttpMethod.Get, uri, formData, headers, token);
    }

    protected async IAsyncEnumerable<string> GetStream(
        string uri,
        Dictionary<string, string> headers = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        using var request = PrepareRequest(HttpMethod.Get, uri, null, headers);
        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        await using var body = await response.Content.ReadAsStreamAsync(token);
        using var reader = new StreamReader(body);
        while (!reader.EndOfStream)
            yield return await reader.ReadLineAsync(token);
    }

    protected async IAsyncEnumerable<string> PostStream(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        using var request = PrepareRequest(HttpMethod.Post, uri, body, headers);
        using var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        await using var responseBody = await response.Content.ReadAsStreamAsync(token);
        using var reader = new StreamReader(responseBody);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(token);
            yield return line;
        }
    }

    protected async Task<Result<Stream>> GetFileStream(
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        var ms = new MemoryStream();
        using var request = PrepareRequest(HttpMethod.Get, uri, null, headers);
        using var response = await Client.SendAsync(request, token);
        await (await response.Content.ReadAsStreamAsync(token)).CopyToAsync(ms, token);
        return response.IsSuccessStatusCode
            ? Result<Stream>.Ok(ms)
            : Result<Stream>.Failed("Reading stream failed", (int)response.StatusCode);
    }

    protected async Task<Result> Post(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        using var request = PrepareRequest(HttpMethod.Post, uri, body, headers);
        var res = await Call(request, token);
        return res.Success ? Result.Ok() : Result.Failed(res);
    }

    protected Task<Result<T>> Post<T>(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Post, body, uri, headers, token);
    }

    protected Task<Result<T>> Put<T>(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Put, body, uri, headers, token);
    }

    protected Task<Result<T>> Patch<T>(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Patch, body, uri, headers, token);
    }


    protected Task<Result<T>> PostForm<T>(
        string uri,
        Dictionary<string, object> formData = null,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return CallForm<T>(HttpMethod.Post, uri, formData, headers, token);
    }

    protected async Task<Result<T>> Call<T>(
        HttpMethod method,
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        using var request = PrepareRequest(method, uri, body, headers);
        return await Call<T>(request, token);
    }

    private async Task<Result<T>> CallForm<T>(
        HttpMethod method,
        string uri,
        Dictionary<string, object> formData,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        using var request = PrepareFormRequest(method, uri, formData, headers);
        return await Call<T>(request, token);
    }

    protected async Task<Result<T>> Call<T>(HttpRequestMessage request, CancellationToken token = default)
    {
        var res = await Call(request, token);
        if (!res.Success)
            return Result<T>.Failed(res);

        var content = res.Data;
        var data = string.IsNullOrWhiteSpace(content) || content == "[]" ? default : content.FromJson<T>();

        return Result<T>.Ok(data);
    }

    private async Task<Result<string>> Call(HttpRequestMessage request, CancellationToken token = default)
    {
        using var response = await Client.SendAsync(request, token);
        var content = await response.Content.ReadAsStringAsync(token);
        //_logger.LogInformation("Respones: {Res}", content);
        if (response.IsSuccessStatusCode)
        {
            return Result<string>.Ok(content);
        }

        _logger.LogWarning("{Uri} call failed: {Response} {Code}", request.RequestUri, content, response.StatusCode);
        return new Result<string>
        {
            StatusCode = (int)response.StatusCode,
            Message = $"External service call failed: status code {response.StatusCode}"
        };
    }

    private HttpRequestMessage PrepareRequest(
        HttpMethod method,
        string uri,
        object body = null,
        Dictionary<string, string> headers = null)
    {
        var jsonBody = SerializeBody(body);
        var request = new HttpRequestMessage(method, uri);
        if (headers != null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (body != null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        return request;
    }

    private static HttpRequestMessage PrepareFormRequest(
        HttpMethod method,
        string uri,
        Dictionary<string, object> formData,
        Dictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (headers != null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (formData is null) return request;

        var formContent = new MultipartFormDataContent();
        foreach (var (key, value) in formData)
            formContent.Add(new StringContent(value?.ToString() ?? string.Empty, Encoding.UTF8), key);

        request.Content = formContent;
        return request;
    }

    protected virtual string SerializeBody(object body)
    {
        return body?.ToJson(StringExtensions.UnsafeJsonOptions);
    }
}