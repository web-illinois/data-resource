using System.Text.Json;

namespace ResourceInformationV2.Search.JsonThinModels {

    public class TagList {
        private readonly JsonSerializerOptions _serializer = new() { PropertyNamingPolicy = new JsonNamingPolicyLowerCase() };
        public List<string> TagItems { get; set; } = [];
        public string Title { get; set; } = "";

        public override string ToString() => JsonSerializer.Serialize(this, _serializer);
    }
}