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

namespace ResourceInformationV2.Function;

public class Publications {
    private readonly ILogger<Publications> _logger;
    private readonly PublicationGetter _publicationGetter;
    private readonly ApiHelper _apiHelper;
    private readonly LogHelper _logHelper;
    private readonly PublicationSetter _publicationSetter;

    public Publications(ILogger<Publications> logger, PublicationGetter publicationGetter, ApiHelper apiHelper, LogHelper logHelper, PublicationSetter publicationSetter) {
        _logger = logger;
        _publicationGetter = publicationGetter;
        _apiHelper = apiHelper;
        _logHelper = logHelper;
        _publicationSetter = publicationSetter;
    }

    [Function("PublicationLoad")]
    [OpenApiOperation(operationId: "PublicationLoad", tags: "Publications", Description = "Load a publication by API. This will be put in draft mode unless instructed by the Administration application.")]
    [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Publication), Required = true, Description = "A json implementation of a publication. An ID will be generated automatically if it isn't created, and it will error out if the ID doesn't start with the source plus a '-' value.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The ID of the publication that was loaded.")]
    public async Task<HttpResponseData> Load([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        try {
            _logger.LogInformation("Called PublicationLoad.");
            var requestHelper = RequestHelperFactory.Create();
            requestHelper.Initialize(req);
            var key = requestHelper.GetCodeFromHeader(req);
            var item = await req.ReadFromJsonAsync<Publication>() ?? new Publication();
            var results = await _apiHelper.CheckApi(item.Source, key);
            if (!results.allowApi) {
                throw new Exception($"API Key in header ilw-key is needed, was sent '{key}'");
            }
            item.Prepare();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results.forceDraft
                ? await _publicationSetter.SetItemWithDraft(item)
                : await _publicationSetter.SetItem(item));
            await _logHelper.Log(CategoryType.Publication, FieldType.None, "API", item.Source, item, "API Load", EmailType.OnSubmission);
            return response;
        } catch (Exception ex) {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(ex.Message);
            return response;
        }
    }

    [Function("PublicationFragment")]
    [OpenApiOperation(operationId: "PublicationFragment", tags: "Publications", Description = "Get a specific publication by a URL-friendly fragment.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiParameter(name: "fragment", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The fragment. If multiple items have the same fragment, this will return the first one it finds.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Publication), Description = "The resource. If the resource is not found, it will be blank.")]
    public async Task<HttpResponseData> GetByFragment([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PublicationFragment.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var fragment = requestHelper.GetRequest(req, "fragment");
        requestHelper.Validate();
        var returnItem = await _publicationGetter.GetItem(source, fragment);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
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
    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called Publication.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var id = requestHelper.GetRequest(req, "id");
        requestHelper.Validate();
        var returnItem = await _publicationGetter.GetItem(id, true);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
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
    [OpenApiParameter(name: "sort", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "Sort order -- defaults to title, but if set to 'date', then it sorts by publication date descending")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Publication>), Description = "The list of publications")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called PublicationSearch.");
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
        var authors = requestHelper.GetArray(req, "authors");
        var status = requestHelper.GetRequest(req, "status", false);
        var query = requestHelper.GetRequest(req, "q", false);
        var take = requestHelper.GetInteger(req, "take", 1000);
        var skip = requestHelper.GetInteger(req, "skip");
        var sort = requestHelper.GetRequest(req, "sort", false).ToLowerInvariant();

        requestHelper.Validate();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _publicationGetter.SearchPublications(source, query, tags, tags2, tags3, tags4, topics, audience, department, authors, status, take, skip, sort));
        return response;
    }
}