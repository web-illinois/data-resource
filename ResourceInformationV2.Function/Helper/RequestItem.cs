namespace ResourceInformationV2.Function.Helper {

    public class RequestItem {
        public string Body { get; set; } = "";
        public string Host { get; set; } = "";
        public List<Tuple<string, string>> Parameters { get; set; } = [];
        public string ParameterString => string.Join("; ", Parameters.Select(i => i.Item1 + "=\"" + i.Item2 + "\""));
        public string Path { get; set; } = "";
        public string Referrer { get; set; } = "";
        public DateTime StartDate { get; set; }
    }
}