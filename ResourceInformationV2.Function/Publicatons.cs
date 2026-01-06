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
using System.Net;

namespace ResourceInformationV2.Function;

public class Publications
{
    private readonly ILogger<Publications> _logger;
    private readonly PublicationGetter _publicationGetter;

    public Publications(ILogger<Publications> logger, PublicationGetter publicationGetter)
    {
        _logger = logger;
        _publicationGetter = publicationGetter;
    }

    [Function("PublicationFragment")]
    [OpenApiOperation(operationId: "PublicationFragment", tags: "Publications", Description = "Get a specific publication by a URL-friendly fragment.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiParameter(name: "fragment", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The fragment. If multiple items have the same fragment, this will return the first one it finds.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Publication), Description = "The resource. If the resource is not found, it will be blank.")]
    public async Task<HttpResponseData> GetByFragment([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Called PublicationFragment.");
        RequestHelper requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var fragment = requestHelper.GetRequest(req, "fragment");
        requestHelper.Validate();
        Publication returnItem = await _publicationGetter.GetItem(source, fragment);
        returnItem.PrepareForExport();
        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("GetPublicationsByFragment")]
    [OpenApiOperation(operationId: "GetPublicationsByFragment", tags: "Publications", Description = "A legacy version of PublicationFragment.")]
    public async Task<HttpResponseData> GetByFragmentAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await GetByFragment(req);

    [Function("Publication")]
    [OpenApiOperation(operationId: "Publication", tags: "Publications", Description = "Get a specific publication.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the item (the id includes the source).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Publication), Description = "The publication. If the publication is not found, it will be blank.")]
    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Called Publication.");
        RequestHelper requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var id = requestHelper.GetRequest(req, "id");
        requestHelper.Validate();
        Publication returnItem = await _publicationGetter.GetItem(id, true);
        returnItem.PrepareForExport();
        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("GetPublications")]
    [OpenApiOperation(operationId: "GetPublications", tags: "Publications", Description = "A legacy version of Publication.")]
    public async Task<HttpResponseData> GetByIdAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await GetById(req);

    [Function("PublicationSearch")]
    [OpenApiOperation(operationId: "PublicationSearch", tags: "Publications", Description = "Search publications by a specific source. The search can include both a free-query text search and filter list.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "tag1", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'.")]
    [OpenApiParameter(name: "tag2", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag3", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag4", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "topic", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of topics. You can separate the topics by the characters '[-]'.")]
    [OpenApiParameter(name: "audience", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of audiences. You can separate the audiences by the characters '[-]'.")]
    [OpenApiParameter(name: "department", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of departments. You can separate the departments by the characters '[-]'.")]
    [OpenApiParameter(name: "authors", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of authors. You can separate the authors by the characters '[-]'.")]
    [OpenApiParameter(name: "status", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A status.")]
    [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A full text search string -- it will search the title and description for the search querystring.")]
    [OpenApiParameter(name: "take", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "How many items do you want? Defaults to 1000.")]
    [OpenApiParameter(name: "skip", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "A skip value to help with pagination. Defaults to 0.")]
    [OpenApiParameter(name: "sortorder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Sort order -- defaults to title, but if set to 'date', then it sorts by date descending")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Publication>), Description = "The list of publications")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Called PublicationSearch.");
        RequestHelper requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        IEnumerable<string> tags = requestHelper.GetArray(req, "tag1");
        IEnumerable<string> tags2 = requestHelper.GetArray(req, "tag2");
        IEnumerable<string> tags3 = requestHelper.GetArray(req, "tag3");
        IEnumerable<string> tags4 = requestHelper.GetArray(req, "tag4");
        IEnumerable<string> topics = requestHelper.GetArray(req, "topic");
        IEnumerable<string> audience = requestHelper.GetArray(req, "audience");
        IEnumerable<string> department = requestHelper.GetArray(req, "department");
        IEnumerable<string> authors = requestHelper.GetArray(req, "authors");
        var status = requestHelper.GetRequest(req, "status", false);
        var query = requestHelper.GetRequest(req, "q", false);
        var take = requestHelper.GetInteger(req, "take", 1000);
        var skip = requestHelper.GetInteger(req, "skip");
        var sort = requestHelper.GetRequest(req, "sort", false).ToLowerInvariant();

        requestHelper.Validate();
        HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _publicationGetter.SearchPublications(source, query, tags, tags2, tags3, tags4, topics, audience, department, authors, status, take, skip, sort));
        return response;
    }

    [Function("SearchPublications")]
    [OpenApiOperation(operationId: "SearchPublications", tags: "Publications", Description = "A legacy version of search publication.")]
    public async Task<HttpResponseData> SearchAlt([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) => await Search(req);
}