using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Function.Helper;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;
using ResourceInformationV2.Search.Setters;
using System.Net;
using LogHelper = ResourceInformationV2.Data.DataHelpers.LogHelper;

namespace ResourceInformationV2.Function;

public class Resources(ILogger<Resources> logger, ResourceGetter resourceGetter, ResourceSetter resourceSetter, LinkCheckHelper linkCheckHelper, ApiHelper apiHelper, LogHelper logHelper) {
    private readonly ILogger<Resources> _logger = logger;
    private readonly ResourceGetter _resourceGetter = resourceGetter;
    private readonly ResourceSetter _resourceSetter = resourceSetter;
    private readonly ApiHelper _apiHelper = apiHelper;
    private readonly LinkCheckHelper _linkCheckHelper = linkCheckHelper;
    private readonly LogHelper _logHelper = logHelper;

    [Function("ResourceFragment")]
    [OpenApiOperation(operationId: "ResourceFragment", tags: "Resources", Description = "Get a specific resource by using a URL-friendly fragment.")]
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
        var returnItem = await _resourceGetter.GetItem(source, fragment);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("ResourceLinkCheck")]
    [OpenApiOperation(operationId: "ResourceLinkCheck", tags: "Resources", Description = "Check the status of resource links.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "A list of resources checked along with the status.")]
    public async Task<HttpResponseData> LinkCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called ResourceLinkCheck.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var count = requestHelper.GetInteger(req, "count", 10);
        requestHelper.Validate();
        var returnItem = await _linkCheckHelper.CheckLink(count);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync(returnItem);
        return response;
    }

    [Function("GetResourcesByFragment")]
    [OpenApiOperation(operationId: "GetResourcesByFragment", tags: "Resources", Description = "A legacy version of ResourceFragment.")]
    public async Task<HttpResponseData> GetByFragmentAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await GetByFragment(req);

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
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("GetResources")]
    [OpenApiOperation(operationId: "GetResources", tags: "Resources", Description = "A legacy version of Resource.")]
    public async Task<HttpResponseData> GetByIdAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await GetById(req);

    [Function("ResourceSearch")]
    [OpenApiOperation(operationId: "ResourceSearch", tags: "Resources", Description = "Search resources by a specific source. The search can include both a free-query text search and filter list.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "tag1", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'.")]
    [OpenApiParameter(name: "tag2", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag3", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag4", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "topic", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of topics. You can separate the topics by the characters '[-]'.")]
    [OpenApiParameter(name: "audience", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of audiences. You can separate the audiences by the characters '[-]'.")]
    [OpenApiParameter(name: "department", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of departments. You can separate the departments by the characters '[-]'.")]
    [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A full text search string -- it will search the title and description for the search querystring.")]
    [OpenApiParameter(name: "take", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "How many items do you want? Defaults to 1000.")]
    [OpenApiParameter(name: "skip", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "A skip value to help with pagination. Defaults to 0.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Resource>), Description = "The list of resources")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called ResourceSearch.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var tags = requestHelper.GetArray(req, "tag1");
        var tags2 = requestHelper.GetArray(req, "tag2");
        var tags3 = requestHelper.GetArray(req, "tag3");
        var tags4 = requestHelper.GetArray(req, "tag4");
        var topics = requestHelper.GetArray(req, "topic");
        var audience = requestHelper.GetArray(req, "audience");
        var department = requestHelper.GetArray(req, "department");
        var query = requestHelper.GetRequest(req, "q", false);
        var take = requestHelper.GetInteger(req, "take", 1000);
        var skip = requestHelper.GetInteger(req, "skip");
        var sort = requestHelper.GetRequest(req, "sort", false).ToLowerInvariant();

        requestHelper.Validate();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _resourceGetter.Search(source, query, tags, tags2, tags3, tags4, topics, audience, department, take, skip, sort));
        return response;
    }

    [Function("ResourceLoad")]
    [OpenApiOperation(operationId: "ResourceLoad", tags: "Resources", Description = "Load a resource by API. This will be put in draft mode unless overridden by the Administration application.")]
    [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
    [OpenApiParameter(name: "resource", In = ParameterLocation.Query, Required = false, Type = typeof(Resource), Description = "A json implementation of a resource. An ID will be generated automatically if it isn't created, and it will error out if the ID doesn't start with the source plus a '-' value.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The ID of the resource that was loaded.")]
    public async Task<HttpResponseData> Load([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called ResourceLoad.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var key = requestHelper.GetCodeFromHeader(req);
        var item = await req.ReadFromJsonAsync<Resource>() ?? new Resource();
        var results = await _apiHelper.CheckApi(item.Source, key);
        if (!results.allowApi) {
            throw new Exception($"API Key in header ilw-key is needed, was sent '{key}'");
        }
        item.Prepare();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(results.forceDraft ? await _resourceSetter.SetItemWithDraft(item) : await _resourceSetter.SetItem(item));
        await _logHelper.Log(CategoryType.Resource, FieldType.None, "API", item.Source, item, "API Load", EmailType.OnSubmission);
        return response;
    }


    [Function("SearchResources")]
    [OpenApiOperation(operationId: "SearchResources", tags: "Resources", Description = "A legacy version of ResourceSearch")]
    public async Task<HttpResponseData> SearchAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await Search(req);
}