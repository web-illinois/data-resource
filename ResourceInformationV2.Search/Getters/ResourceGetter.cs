using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class ResourceGetter(OpenSearchClient? openSearchClient) : BaseGetter<Resource>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Resources.ConvertToUrlString(); }
    }
}