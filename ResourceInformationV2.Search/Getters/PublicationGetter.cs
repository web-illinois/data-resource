using OpenSearch.Client;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class PublicationGetter(OpenSearchClient? openSearchClient) : BaseGetter<Publication>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Publications.ConvertToUrlString(); }

        public async Task<SearchObject<Publication>> SearchPublications(string source, string search, IEnumerable<string> tags, IEnumerable<string> tags2, IEnumerable<string> tags3, IEnumerable<string> tags4, IEnumerable<string> topics, IEnumerable<string> audience, IEnumerable<string> authors, string status, int take, int skip) {
            var response = await _openSearchClient.SearchAsync<Publication>(s => s.Index(IndexName)
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
                        f => audience.Any() ? f.Terms(m => m.Field(fld => fld.AudienceList).Terms(audience)) : f.MatchAll(),
                        f => authors.Any() ? f.Terms(m => m.Field(fld => fld.Authors).Terms(authors)) : f.MatchAll(),
                        f => status != "" ? f.Term(m => m.Field(fld => fld.Status).Value(status)) : f.MatchAll())
                    .Must(m => !string.IsNullOrWhiteSpace(search) ? m.MultiMatch(m => m.Fields(fld => fld.Field("title^10").Field("description^5").Field("notes")).Query(search)) : m.MatchAll())))
                    .Sort(srt => srt.Ascending(f => f.TitleSortKeyword))
                    .Suggest(a => a.Phrase("didyoumean", p => p.Text(search).Field(fld => fld.Title))));
            LogDebug(response);

            List<Publication> documents = response.IsValid ? [.. response.Documents] : [];
            return new SearchObject<Publication>() {
                Error = !response.IsValid ? response.ServerError.Error.ToString() : "",
                DidYouMean = response.Suggest["didyoumean"].FirstOrDefault()?.Options?.FirstOrDefault()?.Text ?? "",
                Total = (int) response.Total,
                Items = documents
            };
        }
    }
}