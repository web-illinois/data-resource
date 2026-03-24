using OpenSearch.Client;

namespace ResourceInformationV2.Search.Models {

    public class Event : BaseObject {
        public override string EditLink => EditLinkRoot + "event/" + Id;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public bool IsAllDay { get; set; } = false;
        [Keyword] public string Location { get; set; } = "";
        public override string NameType => "Event";
        [Keyword] public string SpeakerList { get; set; } = "";
        [Keyword] public IEnumerable<string> Speakers { get; set; } = [];
        [Keyword] public string Sponsor { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now;

        internal override string[] Headings => ["Id", "Title", "Description", "Start Date", "End Date", "Is All Day", "Location", "Speaker", "Sponsor", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        public override void Prepare() {
            base.Prepare();
            SpeakerList = string.Join(" ", Speakers);
        }

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Title = PrepareForTextUpload(lineArray[1]);
            Description = PrepareForTextUpload(lineArray[2]);
            _ = DateTime.TryParse(PrepareForTextUpload(lineArray[3]), out var startDate);
            StartDate = startDate == default ? DateTime.Now : startDate;
            _ = DateTime.TryParse(PrepareForTextUpload(lineArray[4]), out var endDate);
            EndDate = endDate == default ? DateTime.Now : endDate;
            _ = bool.TryParse(lineArray[5], out var isAllDay);
            IsAllDay = isAllDay;
            Location = PrepareForTextUpload(lineArray[6]);
            Speakers = GetTagsFromString(lineArray[7]);
            Sponsor = PrepareForTextUpload(lineArray[8]);
            Fragment = lineArray[9];
            Url = lineArray[10];
            Image = lineArray[11];
            ImageAltText = lineArray[12];
            ImageSource = lineArray[13];
            VideoUrl = lineArray[14];
            Notes = PrepareForTextUpload(lineArray[15]);
            _ = DateTime.TryParse(lineArray[16], out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[17], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[18]);
            DepartmentList = GetTagsFromString(lineArray[19]);
            TopicList = GetTagsFromString(lineArray[20]);
            TagList = GetTagsFromString(lineArray[21]);
            Tag2List = GetTagsFromString(lineArray[22]);
            Tag3List = GetTagsFromString(lineArray[23]);
            Tag4List = GetTagsFromString(lineArray[24]);
            LinkList = GetLinksFromString(lineArray[25]);
            _ = int.TryParse(lineArray[26], out var order);
            Order = order;
            ReviewEmail = lineArray[27];
            _ = DateTime.TryParse(lineArray[28], out var lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            Title,
            PrepareForTextDownload(Description),
            StartDate.ToString("g"),
            EndDate.ToString("g"),
            IsAllDay.ToString(),
            Location,
            string.Join(";", Speakers),
            Sponsor,
            Fragment,
            Url,
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
            LinkList == null ? "" : string.Join(";", LinkList.Select(link => link.ToString())),
            Order.ToString(),
            ReviewEmail,
            LastUpdated.ToString("g")
        ];
    }
}