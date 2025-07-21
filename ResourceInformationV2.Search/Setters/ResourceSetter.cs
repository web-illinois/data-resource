using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class ResourceSetter(OpenSearchClient? openSearchClient) : BaseSetter<Resource>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Resources.ConvertToUrlString(); }
    }
}