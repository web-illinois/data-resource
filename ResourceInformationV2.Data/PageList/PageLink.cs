namespace ResourceInformationV2.Data.PageList {

    public class PageLink(string text, string url) {
        public bool IsCurrent = false;
        public bool IsEmphasized = false;
        public string Text { get; set; } = text;
        public string Url { get; set; } = url;
    }
}