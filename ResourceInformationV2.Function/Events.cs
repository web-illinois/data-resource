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

public class Events {
    private readonly EventGetter _eventGetter;
    private readonly ILogger<Events> _logger;

    public Events(ILogger<Events> logger, EventGetter eventGetter) {
        _logger = logger;
        _eventGetter = eventGetter;
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
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiParameter(name: "location", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The location for the event.")]
    [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The start date for the event.")]
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