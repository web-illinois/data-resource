using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class EventSetter(OpenSearchClient? openSearchClient) : BaseSetter<Event>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Events.ConvertToUrlString(); }
    }
}