namespace ResourceInformationV2.Search.Models {

    public class Publication : BaseObject {
        public IEnumerable<string> Authors { get; set; } = [];
        public string BookTitle { get; set; } = "";
        public string Doi { get; set; } = "";
        public override string EditLink => _editLink + "publication/" + Id;
        public string Issue { get; set; } = "";
        public string JournalName { get; set; } = "";
        public override string NameType => "Publication";
        public string PageNumbers { get; set; } = "";
        public string PublicationType { get; set; } = "";
        public string PublishedDate { get; set; } = "";
        public string PublishedDateNumeric => DateTime.TryParse(PublishedDate, out var date) ? date.ToString("yyyyMMdd") : "";
        public string Status { get; set; } = "";
        public string Volume { get; set; } = "";
    }
}