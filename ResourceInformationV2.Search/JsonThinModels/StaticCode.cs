namespace ResourceInformationV2.Search.JsonThinModels {

    public class StaticCode {
        public List<string> Codes { get; set; } = [];

        public string CodeString { get; set; } = "";

        public int OrderBy { get; set; }

        public string Title { get; set; } = "";
    }
}