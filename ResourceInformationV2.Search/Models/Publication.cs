using OpenSearch.Client;

namespace ResourceInformationV2.Search.Models {

    public class Publication : BaseObject {
        [Keyword] public IEnumerable<string> Authors { get; set; } = [];
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

        internal override string[] Headings => ["Id", "Title", "Description", "Fragment", "Link Url", "Link Text", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Authors", "Book Title", "Issue", "Journal Name", "Page Numbers", "Publication Type", "PublishedDate", "Status", "Volume", "DOI", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Title = PrepareForTextUpload(lineArray[1]);
            Description = PrepareForTextUpload(lineArray[2]);
            Fragment = lineArray[3];
            Url = lineArray[4];
            UrlText = lineArray[5];
            Image = lineArray[6];
            ImageAltText = lineArray[7];
            ImageSource = lineArray[8];
            VideoUrl = lineArray[9];
            Notes = PrepareForTextUpload(lineArray[10]);
            _ = DateTime.TryParse(PrepareForTextUpload(lineArray[11]), out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[12], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[13]);
            DepartmentList = GetTagsFromString(lineArray[14]);
            TopicList = GetTagsFromString(lineArray[15]);
            TagList = GetTagsFromString(lineArray[16]);
            Tag2List = GetTagsFromString(lineArray[17]);
            Tag3List = GetTagsFromString(lineArray[18]);
            Tag4List = GetTagsFromString(lineArray[19]);
            Authors = GetTagsFromString(lineArray[20]);
            BookTitle = PrepareForTextUpload(lineArray[21]);
            Issue = PrepareForTextUpload(lineArray[22]);
            JournalName = PrepareForTextUpload(lineArray[23]);
            PageNumbers = PrepareForTextUpload(lineArray[24]);
            PublicationType = PrepareForTextUpload(lineArray[25]);
            PublishedDate = PrepareForTextUpload(lineArray[26]);
            Status = PrepareForTextUpload(lineArray[27]);
            Volume = PrepareForTextUpload(lineArray[28]);
            Doi = PrepareForTextUpload(lineArray[29]);
            LinkList = GetLinksFromString(lineArray[30]);
            _ = int.TryParse(lineArray[31], out var order);
            Order = order;
            ReviewEmail = lineArray[32];
            _ = DateTime.TryParse(PrepareForTextUpload(lineArray[33]), out var lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            Title,
            PrepareForTextDownload(Description),
            Fragment,
            Url,
            UrlText,
            Image,
            ImageAltText,
            ImageSource,
            VideoUrl,
            PrepareForTextDownload(Notes),
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