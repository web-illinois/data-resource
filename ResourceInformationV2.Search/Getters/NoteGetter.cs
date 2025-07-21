using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Getters {

    public class NoteGetter(OpenSearchClient? openSearchClient) : BaseGetter<NoteItem>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Notes.ConvertToUrlString(); }
    }
}