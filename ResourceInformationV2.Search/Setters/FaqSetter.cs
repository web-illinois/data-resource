using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class FaqSetter(OpenSearchClient? openSearchClient) : BaseSetter<FaqItem>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Faqs.ConvertToUrlString(); }
    }
}