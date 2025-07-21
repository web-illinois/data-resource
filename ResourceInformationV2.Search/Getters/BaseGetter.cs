using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public abstract class BaseGetter<T>(OpenSearchClient? openSearchClient) where T : BaseObject, new() {
        internal readonly OpenSearchClient _openSearchClient = openSearchClient ?? default!;

        internal abstract string IndexName { get; }

        public async Task<List<GenericItem>> GetAllItemsBySource(string source, string search) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Size(1000)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)))
                    .Must(m => string.IsNullOrWhiteSpace(search) ? m.MatchAll() : m.Match(m => m.Field(fld => fld.Title).Query(search).Operator(Operator.And))))));
            LogDebug(response);
            return response.IsValid ? [.. response.Documents.Select(r => r.GetGenericItem()).OrderBy(g => g.Title)] : [];
        }

        public async Task<T> GetItem(string id, bool activeOnly = false) {
            if (string.IsNullOrWhiteSpace(id)) {
                return new();
            }
            var response = await _openSearchClient.GetAsync<T>(id);
            LogDebug(response);
            return !response.IsValid || response.Source == null
                ? new()
                : activeOnly && !response.Source.IsActive
                ? new()
                : response.Source;
        }

        public async Task<T> GetItem(string source, string fragment) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                            f => f.Term(m => m.Field(fld => fld.IsActive).Value(true)),
                            f => f.Term(m => m.Field(fld => fld.Fragment).Value(fragment))))));
            LogDebug(response);
            return response.IsValid ? response.Documents?.FirstOrDefault() ?? new() : new();
        }

        internal void LogDebug(ISearchResponse<T> response) {
            if (_openSearchClient.ConnectionSettings.DisableDirectStreaming) {
                Console.WriteLine(response.DebugInformation);
            }
        }

        internal void LogDebug(GetResponse<T> response) {
            if (_openSearchClient.ConnectionSettings.DisableDirectStreaming) {
                Console.WriteLine(response.DebugInformation);
            }
        }
    }
}