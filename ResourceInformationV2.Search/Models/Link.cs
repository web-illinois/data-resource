namespace ResourceInformationV2.Search.Models {

    public class Link {
        public string LinkHref { get; set; } = "";
        public int Order { get; set; }
        public string Title { get; set; } = "";

        public static Link Translate(string s, int i) =>
            new() {
                Title = s.Split(']')[0].Trim(['[', ' ']),
                LinkHref = s.Split(']')[1].Trim(['[', ' ', '(', ')']),
                Order = i + 1
            };

        public override string ToString() => $"[{Title}]({LinkHref})";
    }
}