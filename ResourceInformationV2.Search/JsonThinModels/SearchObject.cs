using System.Text.Json;

namespace ResourceInformationV2.Search.JsonThinModels {

    public class SearchObject<T> {
        private readonly JsonSerializerOptions _serializer = new() { PropertyNamingPolicy = new JsonNamingPolicyLowerCase() };
        public string DidYouMean { get; set; } = "";
        public string Error { get; set; } = "";
        public List<T> Items { get; set; } = [];
        public int Total { get; set; } = 0;

        public override string ToString() => JsonSerializer.Serialize(this, _serializer);
    }
}