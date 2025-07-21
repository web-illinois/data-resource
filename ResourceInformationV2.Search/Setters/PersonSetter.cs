using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class PersonSetter(OpenSearchClient? openSearchClient) : BaseSetter<Person>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.People.ConvertToUrlString(); }
    }
}