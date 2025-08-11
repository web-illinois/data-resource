using System.Text;
using OpenSearch.Client;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public abstract class BaseGetter<T>(OpenSearchClient? openSearchClient) where T : BaseObject, new() {
        internal readonly OpenSearchClient _openSearchClient = openSearchClient ?? default!;

        internal abstract string IndexName { get; }

        public async Task<string> DownloadFile(string source) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                .Size(10000)
                .Query(q => q
                .Bool(b => b
                .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)))
                .Must(m => m.MatchAll()))));
            LogDebug(response);
            var sb = new StringBuilder();
            _ = sb.AppendLine(string.Join('\t', new T().Headings));
            foreach (var document in response.Documents) {
                _ = sb.AppendLine(string.Join('\t', document.SaveToStringPrivate()));
            }
            return sb.ToString();
        }

        public async Task<List<GenericItem>> GetAllDraftItemsBySource(string source) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Size(10000)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                            f => f.Term(m => m.Field(fld => fld.IsActive).Value(false)))
                    .Must(m => m.MatchAll()))));
            LogDebug(response);
            return response.IsValid ? [.. response.Documents.Select(r => r.GetGenericItem()).OrderBy(g => g.Title)] : [];
        }

        public async Task<List<GenericItem>> GetAllItemsBySource(string source, string search) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Size(10000)
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

        public async Task<SearchObject<T>> Search(string source, string search, IEnumerable<string> tags, IEnumerable<string> tags2, IEnumerable<string> tags3, IEnumerable<string> tags4, IEnumerable<string> topics, IEnumerable<string> audience, int take, int skip) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Skip(skip)
                    .Size(take)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                        f => f.Term(m => m.Field(fld => fld.IsActive).Value(true)),
                        f => tags.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags)) : f.MatchAll(),
                        f => tags2.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags2)) : f.MatchAll(),
                        f => tags3.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags3)) : f.MatchAll(),
                        f => tags4.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags4)) : f.MatchAll(),
                        f => topics.Any() ? f.Term(m => m.Field(fld => fld.TopicList).Value(topics)) : f.MatchAll(),
                        f => audience.Any() ? f.Terms(m => m.Field(fld => fld.AudienceList).Terms(audience)) : f.MatchAll())
                    .Must(m => !string.IsNullOrWhiteSpace(search) ? m.MultiMatch(m => m.Fields(fld => fld.Field("title^10").Field("description^5").Field("notes")).Query(search)) : m.MatchAll())))
                    .Sort(srt => srt.Ascending(f => f.TitleSortKeyword))
                    .Suggest(a => a.Phrase("didyoumean", p => p.Text(search).Field(fld => fld.Title))));
            LogDebug(response);

            List<T> documents = response.IsValid ? [.. response.Documents] : [];
            return new SearchObject<T>() {
                Error = !response.IsValid ? response.ServerError.Error.ToString() : "",
                DidYouMean = response.Suggest["didyoumean"].FirstOrDefault()?.Options?.FirstOrDefault()?.Text ?? "",
                Total = (int) response.Total,
                Items = documents
            };
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