using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class PublicationGetter(OpenSearchClient? openSearchClient) : BaseGetter<Publication>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Publications.ConvertToUrlString(); }
    }
}