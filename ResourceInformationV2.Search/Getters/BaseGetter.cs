using OpenSearch.Client;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;
using System.Text;

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

        public async Task<List<TagList>> GetTagCount(string source) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                .Size(0)
                .Aggregations(a => a
                .Terms("department", t => t.Field(f => f.DepartmentList))
                .Terms("tags1", t => t.Field(f => f.TagList))
                .Terms("tags2", t => t.Field(f => f.Tag2List))
                .Terms("tags3", t => t.Field(f => f.Tag3List))
                .Terms("tags4", t => t.Field(f => f.Tag4List))
                .Terms("audience", t => t.Field(f => f.AudienceList))
                .Terms("topic", t => t.Field(f => f.TopicList)))
                .Query(q => q
                .Bool(b => b
                .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)))
                .Must(m => m.MatchAll()))));
            LogDebug(response);

            var returnValue = new List<TagList> {
                new() { Title = "Tag 1", TagItems = ((BucketAggregate) response.Aggregations["tags1"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Tag 2", TagItems = ((BucketAggregate) response.Aggregations["tags2"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Tag 3", TagItems = ((BucketAggregate) response.Aggregations["tags3"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Tag 4", TagItems = ((BucketAggregate) response.Aggregations["tags4"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Audience", TagItems = ((BucketAggregate) response.Aggregations["audience"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Topic", TagItems = ((BucketAggregate) response.Aggregations["topic"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() },
                new() { Title = "Department", TagItems = ((BucketAggregate) response.Aggregations["department"]).Items.Select(i => ConvertKeyedBucketToString((KeyedBucket<object>) i)).OrderBy(t => t).ToList() }
            };
            return [.. returnValue.Where(tl => tl.TagItems.Count > 0)];
        }

        public async Task<SearchObject<T>> Search(string source, string search, IEnumerable<string> tags, IEnumerable<string> tags2, IEnumerable<string> tags3, IEnumerable<string> tags4, IEnumerable<string> topics, IEnumerable<string> audience, IEnumerable<string> departments, int take, int skip, string sort, bool isActive = true) {
            var response = await _openSearchClient.SearchAsync<T>(s => s.Index(IndexName)
                    .Skip(skip)
                    .Size(take)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                        f => f.Term(m => m.Field(fld => fld.IsActive).Value(isActive)),
                        f => tags.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags)) : f.MatchAll(),
                        f => tags2.Any() ? f.Terms(m => m.Field(fld => fld.Tag2List).Terms(tags2)) : f.MatchAll(),
                        f => tags3.Any() ? f.Terms(m => m.Field(fld => fld.Tag3List).Terms(tags3)) : f.MatchAll(),
                        f => tags4.Any() ? f.Terms(m => m.Field(fld => fld.Tag4List).Terms(tags4)) : f.MatchAll(),
                        f => topics.Any() ? f.Terms(m => m.Field(fld => fld.TopicList).Terms(topics)) : f.MatchAll(),
                        f => audience.Any() ? f.Terms(m => m.Field(fld => fld.AudienceList).Terms(audience)) : f.MatchAll(),
                        f => departments.Any() ? f.Terms(m => m.Field(fld => fld.DepartmentList).Terms(departments)) : f.MatchAll())
                    .Must(m => !string.IsNullOrWhiteSpace(search) ? m.MultiMatch(m => m.Fields(fld => fld.Field("title^10").Field("description^5").Field("notes")).Query(search)) : m.MatchAll())))
                    .Sort(srt => sort == "date" ? srt.Descending(f => f.CreatedOn) : srt.Ascending(f => f.TitleSortKeyword))
                    .Suggest(a => a.Phrase("didyoumean", p => p.Text(search).Field(fld => fld.Title))));
            LogDebug(response);

            List<T> documents = response.IsValid ? [.. response.Documents] : [];
            documents.ForEach(d => d.PrepareForExport());
            return new SearchObject<T>() {
                Error = !response.IsValid ? response.ServerError.Error.ToString() : "",
                DidYouMean = response.Suggest["didyoumean"].FirstOrDefault()?.Options?.FirstOrDefault()?.Text ?? "",
                Total = (int)response.Total,
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

        private static string ConvertKeyedBucketToString(KeyedBucket<object> bucket) {
            return $"{bucket.Key} (number of items: {bucket.DocCount})";
        }
    }
}