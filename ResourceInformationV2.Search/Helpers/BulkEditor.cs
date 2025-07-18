using OpenSearch.Net;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Helpers {

    public class BulkEditor(OpenSearchLowLevelClient? openSearchLowLevelClient) {
        private readonly OpenSearchLowLevelClient _openSearchLowLevelClient = openSearchLowLevelClient ?? default!;

        public async Task<string> DeleteAllItems(string source) {
            var responseResources = await _openSearchLowLevelClient.DeleteByQueryAsync<StringResponse>(UrlTypes.Resources.ConvertToUrlString(), GenerateDeleteJson(source));
            var responsePublications = await _openSearchLowLevelClient.DeleteByQueryAsync<StringResponse>(UrlTypes.Publications.ConvertToUrlString(), GenerateDeleteJson(source));
            var responsePeople = await _openSearchLowLevelClient.DeleteByQueryAsync<StringResponse>(UrlTypes.People.ConvertToUrlString(), GenerateDeleteJson(source));
            var responseNotes = await _openSearchLowLevelClient.DeleteByQueryAsync<StringResponse>(UrlTypes.Notes.ConvertToUrlString(), GenerateDeleteJson(source));
            var responseFaqs = await _openSearchLowLevelClient.DeleteByQueryAsync<StringResponse>(UrlTypes.Faq.ConvertToUrlString(), GenerateDeleteJson(source));
            return responseResources.Success && responsePublications.Success && responsePeople.Success && responseNotes.Success && responseFaqs.Success ? source + " deleted" : source + " not deleted";
        }

        public async Task<string> DeleteTags(string source, string listName, string oldTag) => await UpdateTags(source, listName, oldTag, "");

        public async Task<string> UpdateTags(string source, string listName, string oldTag, string newTag) {
            var responseResources = await _openSearchLowLevelClient.UpdateByQueryAsync<StringResponse>(UrlTypes.Resources.ConvertToUrlString(), GenerateUpdateJson(source, listName, oldTag, newTag));
            var responsePublications = await _openSearchLowLevelClient.UpdateByQueryAsync<StringResponse>(UrlTypes.Publications.ConvertToUrlString(), GenerateUpdateJson(source, listName, oldTag, newTag));
            var responsePeople = await _openSearchLowLevelClient.UpdateByQueryAsync<StringResponse>(UrlTypes.People.ConvertToUrlString(), GenerateUpdateJson(source, listName, oldTag, newTag));
            var responseNotes = await _openSearchLowLevelClient.UpdateByQueryAsync<StringResponse>(UrlTypes.Notes.ConvertToUrlString(), GenerateUpdateJson(source, listName, oldTag, newTag));
            var responseFaqs = await _openSearchLowLevelClient.UpdateByQueryAsync<StringResponse>(UrlTypes.Faq.ConvertToUrlString(), GenerateUpdateJson(source, listName, oldTag, newTag));
            return responseResources.Success && responsePublications.Success && responsePeople.Success && responseNotes.Success && responseFaqs.Success ? $"{source} updated: tag {oldTag} to {newTag}" : source + " not updated";
        }

        private string GenerateDeleteJson(string sourceCode) => "{\"query\": { \"bool\": { \"must\": { \"match_all\": { } }, \"filter\": [ { \"bool\": { \"must\": [ { \"term\": { \"source\":  \"" + sourceCode + "\" } } ] } } ] } } }";

        private string GenerateUpdateJson(string sourceCode, string listName, string oldText, string newText) => "{ \"script\": { \"source\": \"int i = 0; for (elem in ctx._source." + listName + ") { if (elem == '" + Sanitize(oldText) + "') { ctx._source." + listName + "[i] = '" + Sanitize(newText) + "' } i++; }\", \"lang\": \"painless\" }, \"query\": { \"bool\": { \"filter\": [ { \"term\": { \"source\": \"" + sourceCode + "\" } }, { \"term\": { \"" + listName + "\": \"" + Sanitize(oldText) + "\" } } ] } } }";

        private string Sanitize(string value) => value.Replace("'", "\\\\`");
    }
}