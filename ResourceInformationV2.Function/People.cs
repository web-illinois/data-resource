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
using FieldType = ResourceInformationV2.Data.DataModels.FieldType;
using LogHelper = ResourceInformationV2.Data.DataHelpers.LogHelper;

namespace ResourceInformationV2.Function;

public class People {
    private readonly ApiHelper _apiHelper;
    private readonly ILogger<People> _logger;
    private readonly LogHelper _logHelper;
    private readonly PersonGetter _personGetter;
    private readonly PersonSetter _personSetter;

    public People(ILogger<People> logger, PersonGetter personGetter, PersonSetter personSetter, ApiHelper apiHelper, LogHelper logHelper) {
        _logger = logger;
        _personGetter = personGetter;
        _personSetter = personSetter;
        _apiHelper = apiHelper;
        _logHelper = logHelper;
    }

    [Function("PersonFragment")]
    [OpenApiOperation(operationId: "PersonFragment", tags: "People", Description = "Get a specific person by a URL-friendly fragment.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiParameter(name: "fragment", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The fragment. If multiple items have the same fragment, this will return the first one it finds.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Person), Description = "The person. If the person is not found, it will be blank.")]
    public async Task<HttpResponseData> GetByFragment([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PersonFragment.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var fragment = requestHelper.GetRequest(req, "fragment");
        requestHelper.Validate();
        var returnItem = await _personGetter.GetItem(source, fragment);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("Person")]
    [OpenApiOperation(operationId: "Person", tags: "People", Description = "Get a specific person.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the item (the id includes the source).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Person), Description = "The person. If the person is not found, it will be blank.")]
    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called Person.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var id = requestHelper.GetRequest(req, "id");
        requestHelper.Validate();
        var returnItem = await _personGetter.GetItem(id, true);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("PersonLoad")]
    [OpenApiOperation(operationId: "PersonLoad", tags: "People", Description = "Load a person by API. This will be put in draft mode unless instructed by the Administration application.")]
    [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
    [OpenApiParameter(name: "person", In = ParameterLocation.Query, Required = false, Type = typeof(Person), Description = "A json implementation of a person. An ID will be generated automatically if it isn't created, and it will error out if the ID doesn't start with the source plus a '-' value.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The ID of the person that was loaded.")]
    public async Task<HttpResponseData> Load([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PersonLoad.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var key = requestHelper.GetCodeFromHeader(req);
        var item = await req.ReadFromJsonAsync<Person>() ?? new Person();
        var results = await _apiHelper.CheckApi(item.Source, key);
        if (!results.allowApi) {
            throw new Exception($"API Key in header ilw-key is needed, was sent '{key}'");
        }
        item.Prepare();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(results.forceDraft ? await _personSetter.SetItemWithDraft(item) : await _personSetter.SetItem(item));
        await _logHelper.Log(CategoryType.Person, FieldType.None, "API", item.Source, item, "API Load", EmailType.OnSubmission);
        return response;
    }

    [Function("PersonSearch")]
    [OpenApiOperation(operationId: "PersonSearch", tags: "People", Description = "Search people by a specific source. The search can include both a free-query text search and filter list.")]
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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Person>), Description = "The list of people")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PersonSearch.");
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
        await response.WriteAsJsonAsync(await _personGetter.Search(source, query, tags, tags2, tags3, tags4, topics, audience, department, take, skip, sort));
        return response;
    }

    [Function("PersonSearchInactive")]
    [OpenApiOperation(operationId: "PersonSearchAll", tags: "People", Description = "Search people by a specific source, including inactive items. The search can include both a free-query text search and filter list.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Person>), Description = "The list of people")]
    public async Task<HttpResponseData> SearchAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PersonSearch.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var key = requestHelper.GetCodeFromHeader(req);
        var results = await _apiHelper.CheckApi(source, key);
        if (!results.allowApi) {
            throw new Exception("API Key in header ilw-key is needed, was sent " + key);
        }
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
        var sort = requestHelper.GetRequest(req, "sort").ToLowerInvariant();

        requestHelper.Validate();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _personGetter.Search(source, query, tags, tags2, tags3, tags4, topics, audience, department, take, skip, sort, false));
        return response;
    }
}