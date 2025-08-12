using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Function.Helper;
using ResourceInformationV2.Search.JsonThinModels;

namespace ResourceInformationV2.Function;

public class Tags {
    private readonly FilterHelper _filterHelper;
    private readonly FilterTranslator _filterTranslator;
    private readonly ILogger<Tags> _logger;

    public Tags(ILogger<Tags> logger, FilterHelper filterHelper, FilterTranslator filterTranslator) {
        _logger = logger;
        _filterHelper = filterHelper;
        _filterTranslator = filterTranslator;
    }

    [Function("GetTags")]
    [OpenApiOperation(operationId: "GetTags", tags: "Get Tags", Description = "Get all tags for a specific source.")]
    [OpenApiParameter(name: "source", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The source for the tags.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<StaticCode>), Description = "The list of tags")]
    public async Task<HttpResponseData> Tag([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
        _logger.LogInformation("Called GetTags.");
        var requestHelper = RequestHelperFactory.Create();
        requestHelper.Initialize(req);
        var source = requestHelper.GetRequest(req, "source");
        var response = req.CreateResponse(HttpStatusCode.OK);
        var codes = await _filterHelper.GetAllFilters(source);
        var codeTitles = await _filterHelper.GetTagTitles(source);
        var returnValue = new List<StaticCode>();
        int i = 0;
        foreach (var codeTitle in codeTitles) {
            var tagType = (TagType) Enum.Parse(typeof(TagType), codeTitle.Item1, true);
            var code = codes.FirstOrDefault(codes => codes.Key == tagType);
            if (code != null) {
                returnValue.Add(FilterTranslator.TranslateTags(source, code, codeTitle.Item2, codeTitle.Item1, i++));
            }
        }
        await response.WriteAsJsonAsync(returnValue);
        return response;
    }
}