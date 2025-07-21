using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public abstract class BaseSetter<T>(OpenSearchClient? openSearchClient) where T : BaseObject, new() {
        internal readonly OpenSearchClient _openSearchClient = openSearchClient ?? default!;

        internal abstract string IndexName { get; }

        public async Task<string> DeleteItem(string id) {
            var response = await _openSearchClient.DeleteAsync<T>(id, i => i.Index(IndexName));
            return response.IsValid ? id : "";
        }

        public async Task<string> SetItem(T item) {
            item.Prepare();
            var response = await _openSearchClient.IndexAsync(item, i => i.Index(IndexName));
            return response.IsValid ? item.Id : "";
        }
    }
}