using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ResourceInformationV2.Function;

public class Tags {
    private readonly ILogger<Tags> _logger;

    public Tags(ILogger<Tags> logger) {
        _logger = logger;
    }

    [Function("GetTags")]
    [OpenApiOperation(operationId: "GetTags", tags: "Get Tags", Description = "Get all tags for a specific source.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<TagList>), Description = "The list of tags")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req) {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}