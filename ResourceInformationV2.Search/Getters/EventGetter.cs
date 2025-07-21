using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class EventGetter(OpenSearchClient? openSearchClient) : BaseGetter<Event>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Events.ConvertToUrlString(); }
    }
}