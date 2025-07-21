using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class FaqGetter(OpenSearchClient? openSearchClient) : BaseGetter<FaqItem>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Faqs.ConvertToUrlString(); }
    }
}