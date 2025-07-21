using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class PublicationSetter(OpenSearchClient? openSearchClient) : BaseSetter<Publication>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Publications.ConvertToUrlString(); }
    }
}