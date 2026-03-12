using OpenSearch.Client;
using ResourceInformationV2.Search.JsonThinModels;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class EventGetter(OpenSearchClient? openSearchClient) : BaseGetter<Event>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Events.ConvertToUrlString(); }

        public async Task<SearchObject<Event>> SearchEvents(string source, string search, IEnumerable<string> tags, IEnumerable<string> tags2, IEnumerable<string> tags3, IEnumerable<string> tags4, IEnumerable<string> topics, IEnumerable<string> audience, IEnumerable<string> departments, IEnumerable<string> speakers, string location, string date, int take, int skip, string sort) {
            var response = await _openSearchClient.SearchAsync<Event>(s => s.Index(IndexName)
                    .Skip(skip)
                    .Size(take)
                    .Query(q => q
                    .Bool(b => b
                    .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                        f => f.Term(m => m.Field(fld => fld.IsActive).Value(true)),
                        f => tags.Any() ? f.Terms(m => m.Field(fld => fld.TagList).Terms(tags)) : f.MatchAll(),
                        f => tags2.Any() ? f.Terms(m => m.Field(fld => fld.Tag2List).Terms(tags2)) : f.MatchAll(),
                        f => tags3.Any() ? f.Terms(m => m.Field(fld => fld.Tag3List).Terms(tags3)) : f.MatchAll(),
                        f => tags4.Any() ? f.Terms(m => m.Field(fld => fld.Tag4List).Terms(tags4)) : f.MatchAll(),
                        f => topics.Any() ? f.Term(m => m.Field(fld => fld.TopicList).Value(topics)) : f.MatchAll(),
                        f => audience.Any() ? f.Terms(m => m.Field(fld => fld.AudienceList).Terms(audience)) : f.MatchAll(),
                        f => departments.Any() ? f.Terms(m => m.Field(fld => fld.DepartmentList).Terms(departments)) : f.MatchAll(),
                        f => speakers.Any() ? f.Terms(m => m.Field(fld => fld.Speakers).Terms(speakers)) : f.MatchAll(),
                        f => location != "" ? f.Term(m => m.Field(fld => fld.Location).Value(location)) : f.MatchAll(),
                        f => date != "" ? f.Term(m => m.Field(fld => fld.StartDate).Value(date)) : f.MatchAll())
                    .Must(m => !string.IsNullOrWhiteSpace(search) ? m.MultiMatch(m => m.Fields(fld => fld.Field("title^10").Field("speakers^5").Field("description^5").Field("notes")).Query(search)) : m.MatchAll())))
                    .Sort(srt => sort == "date" ? srt.Ascending(f => f.StartDate) : srt.Ascending(f => f.TitleSortKeyword))
                    .Suggest(a => a.Phrase("didyoumean", p => p.Text(search).Field(fld => fld.Title))));
            LogDebug(response);

            List<Event> documents = response.IsValid ? [.. response.Documents] : [];
            return new SearchObject<Event>() {
                Error = !response.IsValid ? response.ServerError.Error.ToString() : "",
                DidYouMean = response.Suggest["didyoumean"].FirstOrDefault()?.Options?.FirstOrDefault()?.Text ?? "",
                Total = (int)response.Total,
                Items = documents
            };
        }

        public async Task<(IEnumerable<string>, IEnumerable<string>, IEnumerable<string>)> GetEventData(string source) {
            var aggregations = new AggregationDictionary {
                { "speakers", new TermsAggregation("speakers") { Field = "speakers", Size = 100000 } },
                { "sponsor", new TermsAggregation("sponsor") { Field = "sponsor", Size = 100000 } },
                { "locationinformation", new TermsAggregation("locationinformation") { Field = "location", Size = 100000 } }
            };

            var response = await _openSearchClient.SearchAsync<Event>(s => s.Index(IndexName)
                .Size(0)
                .Aggregations(aggregations)
                .Query(q => q
                    .Bool(b => b
                        .Filter(f => f.Term(m => m.Field(fld => fld.Source).Value(source)),
                            f => f.Term(m => m.Field(fld => fld.IsActive).Value(true)))
                        .Must(m => m.MatchAll()))));
            LogDebug(response);
            if (response.IsValid) {
                return (response.Aggregations.Terms("speakers").Buckets.Select(a => a.Key).OrderBy(a => a),
                    response.Aggregations.Terms("sponsor").Buckets.Select(a => a.Key).OrderBy(a => a),
                    response.Aggregations.Terms("locationinformation").Buckets.Select(a => a.Key).OrderBy(a => a));
            }
            return ([], [], []);
        }
    }
}