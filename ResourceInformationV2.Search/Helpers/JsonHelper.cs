using Newtonsoft.Json;
using OpenSearch.Client;
using OpenSearch.Net;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Helpers {

    public class JsonHelper(OpenSearchLowLevelClient? openSearchLowLevelClient, OpenSearchClient? openSearchClient) {
        private readonly OpenSearchClient _openSearchClient = openSearchClient ?? default!;
        private readonly OpenSearchLowLevelClient _openSearchLowLevelClient = openSearchLowLevelClient ?? default!;

        public async Task<string> GetJsonFull(string sourceCode, UrlTypes urltype) {
            var body = await _openSearchLowLevelClient.SearchAsync<StringResponse>(urltype.ConvertToUrlString(), GenerateGetJson(sourceCode));
            dynamic? json = JsonConvert.DeserializeObject(body.Body ?? "");
            return json == null || json?.hits == null || json?.hits.hits == null ? "error" : (string) JsonConvert.SerializeObject(json?.hits.hits);
        }

        public async Task<string> LoadJson(string sourceCode, UrlTypes urltype, string jsonString) {
            var json = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(jsonString ?? "");
            var success = 0;
            var failureIds = new List<string>();
            var useRawJsonItems = false;
            if (json == null) {
                return "error";
            }
            foreach (var jsonItem in json) {
                var body = JsonConvert.SerializeObject(jsonItem._source);
                string id = jsonItem._id.ToString();
                string source = jsonItem._source.source.ToString();
                string altId = jsonItem._source.id.ToString();
                if (id.StartsWith(sourceCode + "-") && altId.StartsWith(sourceCode + "-") && source == sourceCode) {
                    var response = await _openSearchLowLevelClient.IndexAsync<StringResponse>(urltype.ConvertToUrlString(), id, body);
                    if (response.Success) {
                        success++;
                    } else {
                        failureIds.Add(id);
                    }
                } else {
                    failureIds.Add(id);
                }
            }
            return failureIds.Count > 0
                ? $"Loaded {success} items. Failed to load {(failureIds.Count < 5 ? string.Join("; ", failureIds) : failureIds.Count)} items. {(useRawJsonItems ? "Used raw JSON." : "")}"
                : $"Loaded {success} items. {(useRawJsonItems ? "Used raw JSON." : "")}";
        }

        private static string GenerateGetJson(string sourceCode) => "{ \"size\": 10000, \"query\":{\"match\":{\"source\":{\"query\":\"" + sourceCode + "\"}}}}";
    }
}