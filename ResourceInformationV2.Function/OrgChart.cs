using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Function.Helper;
using ResourceInformationV2.Search.Models;
using System.Net;
using System.Text.Json;

namespace ResourceInformationV2.Function;

public class OrgChart {
    private readonly ILogger<OrgChart> _logger;
    private readonly OrgChartHelper _orgChartHelper;

    public OrgChart(ILogger<OrgChart> logger, OrgChartHelper orgChartHelper) {
        _logger = logger;
        _orgChartHelper = orgChartHelper;
    }

    [Function("OrgChart")]
    [OpenApiOperation(operationId: "OrgChart", tags: "OrgChart", Description = "Get a specific organizational chart")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **source** parameter given to you, can use 'test' to test.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Person), Description = "The org chart json. If the org chart is not found, it will be blank.")]

    public async Task<HttpResponseData> GetById([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called OrgChart.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        requestHelper.Validate();
        var returnItem = await _orgChartHelper.GetOrgChartJson(source);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(JsonDocument.Parse(returnItem));
        return response;
    }
}