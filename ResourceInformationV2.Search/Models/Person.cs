namespace ResourceInformationV2.Search.Models {

    public class Person : BaseObject {
        public string DetailText { get; set; } = "";
        public override string EditLink => EditLinkRoot + "person/" + Id;
        public string FirstName { get; set; } = "";
        public string JobLocation { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NameReversed => FirstName + " " + LastName;
        public override string NameType => "Person";
        public override string Title => LastName + ", " + FirstName;

        internal override string[] Headings => ["Id", "First Name", "Last Name", "Job Title", "Job Location", "Biography", "Fragment", "Link Url", "Link Text", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Detail Text", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            FirstName = PrepareForTextUpload(lineArray[1]);
            LastName = PrepareForTextUpload(lineArray[2]);
            JobTitle = PrepareForTextUpload(lineArray[3]);
            JobLocation = PrepareForTextUpload(lineArray[4]);
            Description = PrepareForTextUpload(lineArray[5]);
            Fragment = lineArray[6];
            Url = lineArray[7];
            UrlText = lineArray[8];
            Image = lineArray[9];
            ImageAltText = lineArray[10];
            ImageSource = lineArray[11];
            VideoUrl = lineArray[12];
            Notes = PrepareForTextUpload(lineArray[13]);
            DetailText = PrepareForTextUpload(lineArray[14]);
            _ = DateTime.TryParse(lineArray[15], out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[16], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[17]);
            DepartmentList = GetTagsFromString(lineArray[18]);
            TopicList = GetTagsFromString(lineArray[19]);
            TagList = GetTagsFromString(lineArray[20]);
            Tag2List = GetTagsFromString(lineArray[21]);
            Tag3List = GetTagsFromString(lineArray[22]);
            Tag4List = GetTagsFromString(lineArray[23]);
            LinkList = GetLinksFromString(lineArray[24]);
            _ = int.TryParse(lineArray[25], out var order);
            Order = order;
            ReviewEmail = lineArray[26];
            _ = DateTime.TryParse(lineArray[27], out var lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            FirstName,
            LastName,
            JobTitle,
            JobLocation,
            PrepareForTextDownload(Description),
            Fragment,
            Url,
            UrlText,
            Image,
            ImageAltText,
            ImageSource,
            VideoUrl,
            PrepareForTextDownload(Notes),
            PrepareForTextDownload(DetailText),
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