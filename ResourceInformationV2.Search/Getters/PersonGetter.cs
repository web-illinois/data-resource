using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class PersonGetter(OpenSearchClient? openSearchClient) : BaseGetter<Person>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.People.ConvertToUrlString(); }
    }
}