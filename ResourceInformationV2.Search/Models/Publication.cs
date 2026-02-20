using OpenSearch.Client;

namespace ResourceInformationV2.Search.Models {

    public class Publication : BaseObject {
        public IEnumerable<string> Authors { get; set; } = [];
        public string BookTitle { get; set; } = "";
        [Keyword] public string Doi { get; set; } = "";
        public override string EditLink => EditLinkRoot + "publication/" + Id;
        [Keyword] public string Issue { get; set; } = "";
        public string JournalName { get; set; } = "";
        public override string NameType => "Publication";
        [Keyword] public string PageNumbers { get; set; } = "";
        [Keyword] public string PublicationType { get; set; } = "";
        [Keyword] public string PublishedDate { get; set; } = "";
        [Keyword] public string PublishedDateNumeric => DateTime.TryParse(PublishedDate, out var date) ? date.ToString("yyyyMMdd") : "";
        [Keyword] public string Status { get; set; } = "";
        [Keyword] public string Volume { get; set; } = "";

        internal override string[] Headings => ["Id", "Title", "Description", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Authors", "Book Title", "Issue", "Journal Name", "Page Numbers", "Publication Type", "PublishedDate", "Status", "Volume", "DOI", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Title = lineArray[1];
            Description = lineArray[2];
            Fragment = lineArray[3];
            Url = lineArray[4];
            Image = lineArray[5];
            ImageAltText = lineArray[6];
            ImageSource = lineArray[7];
            VideoUrl = lineArray[8];
            Notes = lineArray[9];
            _ = DateTime.TryParse(lineArray[10], out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[11], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[12]);
            DepartmentList = GetTagsFromString(lineArray[13]);
            TopicList = GetTagsFromString(lineArray[14]);
            TagList = GetTagsFromString(lineArray[15]);
            Tag2List = GetTagsFromString(lineArray[16]);
            Tag3List = GetTagsFromString(lineArray[17]);
            Tag4List = GetTagsFromString(lineArray[18]);
            Authors = GetTagsFromString(lineArray[19]);
            BookTitle = lineArray[20];
            Issue = lineArray[21];
            JournalName = lineArray[22];
            PageNumbers = lineArray[23];
            PublicationType = lineArray[24];
            PublishedDate = lineArray[25];
            Status = lineArray[26];
            Volume = lineArray[27];
            Doi = lineArray[28];
            LinkList = GetLinksFromString(lineArray[29]);
            _ = int.TryParse(lineArray[30], out var order);
            Order = order;
            ReviewEmail = lineArray[31];
            _ = DateTime.TryParse(lineArray[32], out var lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            Title,
            Description,
            Fragment,
            Url,
            Image,
            ImageAltText,
            ImageSource,
            VideoUrl,
            Notes,
            CreatedOn.ToString("g"),
            IsActive.ToString(),
            string.Join(";", AudienceList),
            string.Join(";", DepartmentList),
            string.Join(";", TopicList),
            string.Join(";", TagList),
            string.Join(";", Tag2List),
            string.Join(";", Tag3List),
            string.Join(";", Tag4List),
            string.Join(";", Authors),
            BookTitle,
            Issue,
            JournalName,
            PageNumbers,
            PublicationType,
            PublishedDate,
            Status,
            Volume,
            Doi,
            LinkList == null ? "" : string.Join(";", LinkList.Select(link => link.ToString())),
            Order.ToString(),
            ReviewEmail,
            LastUpdated.ToString("g")
        ];
    }
}