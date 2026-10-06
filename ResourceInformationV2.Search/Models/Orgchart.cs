using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResourceInformationV2.Search.Models {
    public class Orgchart {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Subtitle { get; set; }

        public string Title { get; set; } = "";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Link { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Weight { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Large { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Orgchart>? Children { get; set; }

        [JsonIgnore]
        public Orgchart? Parent { get; set; }

        private static readonly JsonSerializerOptions JsonOptions = new() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };

        public override string ToString() {
            return JsonSerializer.Serialize(this, JsonOptions);
        }

        public static Orgchart? Search(Orgchart root, Orgchart? parent, string searchText) {
            if (string.IsNullOrWhiteSpace(searchText)) {
                return null;
            }

            if ((!string.IsNullOrWhiteSpace(root.Title) && root.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(root.Subtitle) && root.Subtitle.Contains(searchText, StringComparison.OrdinalIgnoreCase))) {
                root.Parent = parent;
                return root;
            }

            foreach (var child in root.Children ?? []) {
                var found = Search(child, root, searchText);
                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        public static Orgchart FromJson(string json) {
            return string.IsNullOrWhiteSpace(json) ? new Orgchart() : JsonSerializer.Deserialize<Orgchart>(json, JsonOptions)!;
        }
    }
}
