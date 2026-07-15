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

public class Events {
    private readonly EventGetter _eventGetter;
    private readonly ILogger<Events> _logger;
    private readonly ApiHelper _apiHelper;
    private readonly LogHelper _logHelper;
    private readonly EventSetter _eventSetter;

    public Events(ILogger<Events> logger, EventGetter eventGetter, ApiHelper apiHelper, LogHelper logHelper, EventSetter eventSetter) {
        _logger = logger;
        _eventGetter = eventGetter;
        _apiHelper = apiHelper;
        _logHelper = logHelper;
        _eventSetter = eventSetter;
    }

    [Function("EventLoad")]
    [OpenApiOperation(operationId: "EventLoad", tags: "Events", Description = "Load an event by API. This will be put in draft mode unless instructed by the Administration application.")]
    [OpenApiParameter(name: "ilw-key", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "The API Key.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(Event), Required = true, Description = "A json implementation of an event. An ID will be generated automatically if it isn't created, and it will error out if the ID doesn't start with the source plus a '-' value.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The ID of the event that was loaded.")]
    public async Task<HttpResponseData> Load([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        try {
            _logger.LogInformation("Called EventLoad.");
            var requestHelper = RequestHelperFactory.Create();
            requestHelper.Initialize(req);
            var key = requestHelper.GetCodeFromHeader(req);
            var item = await req.ReadFromJsonAsync<Event>() ?? new Event();
            var results = await _apiHelper.CheckApi(item.Source, key);
            if (!results.allowApi) {
                throw new Exception($"API Key in header ilw-key is needed, was sent '{key}'");
            }
            item.Prepare();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(results.forceDraft
                ? await _eventSetter.SetItemWithDraft(item)
                : await _eventSetter.SetItem(item));
            await _logHelper.Log(CategoryType.Event, FieldType.None, "API", item.Source, item, "API Load",
                EmailType.OnSubmission);
            return response;
        } catch (Exception ex) {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(ex.Message);
            return response;
        }
    }

    [Function("EventFragment")]
    [OpenApiOperation(operationId: "EventFragment", tags: "Events", Description = "Get a specific event by using a URL-friendly fragment.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiParameter(name: "fragment", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The fragment. If multiple items have the same fragment, this will return the first one it finds.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Event), Description = "The event. If the event is not found, it will be blank.")]
    public async Task<HttpResponseData> GetByFragment([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called EventFragment.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var fragment = requestHelper.GetRequest(req, "fragment");
        requestHelper.Validate();
        var returnItem = await _eventGetter.GetItem(source, fragment);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("EventData")]
    [OpenApiOperation(operationId: "EventData", tags: "Events", Description = "Get data about the events.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(string), Description = "A list of all speakers, sponsors, and locations")]
    public async Task<HttpResponseData> GetEventData([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called EventData.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        requestHelper.Validate();
        var returnItem = await _eventGetter.GetEventData(source);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { speakers = returnItem.Item1, sponsors = returnItem.Item2, locations = returnItem.Item3 });
        return response;
    }

    [Function("Event")]
    [OpenApiOperation(operationId: "Event", tags: "Events", Description = "Get a specific event.")]
    [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The id of the item (the id includes the source).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Event), Description = "The event. If the event is not found, it will be blank.")]
    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called Event.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var id = requestHelper.GetRequest(req, "id");
        requestHelper.Validate();
        var returnItem = await _eventGetter.GetItem(id, true);
        returnItem.PrepareForExport();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(returnItem);
        return response;
    }

    [Function("EventSearch")]
    [OpenApiOperation(operationId: "EventSearch", tags: "Events", Description = "Search events by a specific source. The search can include both a free-query text search and filter list.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "tag1", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'.")]
    [OpenApiParameter(name: "tag2", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag3", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "tag4", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of tags. You can separate the tags by the characters '[-]'. Having multiple tags options allows you to vary the AND and OR options for the tags.")]
    [OpenApiParameter(name: "topic", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of topics. You can separate the topics by the characters '[-]'.")]
    [OpenApiParameter(name: "audience", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of audiences. You can separate the audiences by the characters '[-]'.")]
    [OpenApiParameter(name: "department", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of departments. You can separate the departments by the characters '[-]'.")]
    [OpenApiParameter(name: "speaker", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A list of speakers. You can separate the speakers by the characters '[-]'.")]
    [OpenApiParameter(name: "location", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The location for the event.")]
    [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The start date for the event.")]
    [OpenApiParameter(name: "q", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "A full text search string -- it will search the title and description for the search querystring.")]
    [OpenApiParameter(name: "take", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "How many items do you want? Defaults to 1000.")]
    [OpenApiParameter(name: "skip", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "A skip value to help with pagination. Defaults to 0.")]
    [OpenApiParameter(name: "sort", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "Sort value - either title or date")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(SearchObject<Event>), Description = "The list of events")]
    public async Task<HttpResponseData> Search([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called EventSearch.");
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
        var speakers = requestHelper.GetArray(req, "speaker");
        var location = requestHelper.GetRequest(req, "location", false);
        var date = requestHelper.GetRequest(req, "date", false);
        var take = requestHelper.GetInteger(req, "take", 1000);
        var skip = requestHelper.GetInteger(req, "skip");
        var sort = requestHelper.GetRequest(req, "sort", false).ToLowerInvariant();

        requestHelper.Validate();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(await _eventGetter.SearchEvents(source, query, tags, tags2, tags3, tags4, topics, audience, department, speakers, location, date, take, skip, sort));
        return response;
    }
}