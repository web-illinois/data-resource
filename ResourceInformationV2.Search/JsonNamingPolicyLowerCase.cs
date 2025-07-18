using System.Text.Json;

namespace ResourceInformationV2.Search {

    public class JsonNamingPolicyLowerCase : JsonNamingPolicy {

        public override string ConvertName(string name) => name.ToLower();
    }
}