using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public class NoteSetter(OpenSearchClient? openSearchClient) : BaseSetter<NoteItem>(openSearchClient) {
        internal override string IndexName { get => UrlTypes.Notes.ConvertToUrlString(); }
    }
}