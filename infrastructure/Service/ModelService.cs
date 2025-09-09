using Models.Dtos.Request;
using Models.Dtos.Response;
using Models.Interface;
using Microsoft.Extensions.Configuration;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace infrastructure.Service;

public class ModelService(HttpClient httpClient, IConfiguration config) : IModelService
{

    public async Task<WorkflowResponse> RunWorkflowAsync(WorkflowRequest request)
    {
        var flaskUrl = config["Flask:Url"];
        var flaskToken = config["Flask:Token"];

        var json = JsonSerializer.Serialize(request);
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, flaskUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Add security header (same as Flask API expects)
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", flaskToken);

        var response = await httpClient.SendAsync(httpRequest);
        var rawResponse = await response.Content.ReadAsStringAsync();

        return new WorkflowResponse
        {
            StatusCode = (int)response.StatusCode,
            RawResponse = rawResponse
        };
    }
}

