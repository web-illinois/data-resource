using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResourceInformationV2.Function.Helper;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Function;

public class Resources {
    private readonly ILogger<Resources> _logger;
    private readonly ResourceGetter _resourceGetter;

    public Resources(ILogger<Resources> logger, ResourceGetter resourceGetter) {
        _logger = logger;
        _resourceGetter = resourceGetter;
    }

    [Function("ResourcesFragment")]
    [OpenApiOperation(operationId: "ResourcesFragment", tags: "Resources", Description = "Get all tags for a specific source.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiParameter(name: "fragment", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The fragment. If multiple items have the same fragment, this will return the first one it finds.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Resource), Description = "The resource. If the resource is not found, it will be blank.")]
    public async Task<HttpResponseData> GetByFragment([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called ResourceFragment.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var fragment = requestHelper.GetRequest(req, "fragment");
        requestHelper.Validate();
        var returnItem = (await _resourceGetter.GetItem(source, fragment));
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("Resource")]
    [OpenApiOperation(operationId: "Resource", tags: "Resources", Description = "Get a specific resource.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the item (the id includes the source).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Resource), Description = "The resource. If the resource is not found, it will be blank.")]
    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called Resource.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var id = requestHelper.GetRequest(req, "id");
        requestHelper.Validate();
        var returnItem = await _resourceGetter.GetItem(id, true);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("ResourceSearch")]
    [OpenApiOperation(operationId: "SearchResources", tags: "Resources", Description = "Search resources by a specific source. The search can include both a free-query text search and filter list.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "tags", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'.")]
    [OpenApiParameter(name: "tags2", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tags3", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tags4", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "topics", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of topics. You can separate the skills by the characters '[-]'.")]
    [OpenApiParameter(name: "audiences", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of audiences. You can separate the departments by the characters '[-]'.")]
    [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A full text search string -- it will search the title and description for the search querystring.")]
    [OpenApiParameter(name: "take", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "How many items do you want? Defaults to 1000.")]
    [OpenApiParameter(name: "skip", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "A skip value to help with pagination. Defaults to 0.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Resource>), Description = "The list of resources")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called ResourceSearch.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var tags = requestHelper.GetArray(req, "tags");
        var tags2 = requestHelper.GetArray(req, "tags2");
        var tags3 = requestHelper.GetArray(req, "tags3");
        var tags4 = requestHelper.GetArray(req, "tags4");
        var topics = requestHelper.GetArray(req, "topics");
        var audience = requestHelper.GetArray(req, "audience");
        var query = requestHelper.GetRequest(req, "q", false);
        var take = requestHelper.GetInteger(req, "take", 1000);
        var skip = requestHelper.GetInteger(req, "skip");

        requestHelper.Validate();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _resourceGetter.Search(source, query, tags, tags2, tags3, tags4, topics, audience, take, skip));
        return response;
    }
}