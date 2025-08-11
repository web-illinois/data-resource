namespace ResourceInformationV2.Search.Models {

    public class Person : BaseObject {
        public override string EditLink => _editLink + "person/" + Id;
        public string FirstName { get; set; } = "";
        public string JobLocation { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NameReversed => LastName + ", " + FirstName;
        public override string NameType => "Person";
        public override string Title => FirstName + " " + LastName;

        internal override string[] Headings => ["Id", "First Name", "Last Name", "Job Title", "Job Location", "Biography", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            FirstName = lineArray[1];
            LastName = lineArray[2];
            JobTitle = lineArray[3];
            JobLocation = lineArray[4];
            Description = lineArray[5];
            Fragment = lineArray[6];
            Url = lineArray[7];
            Image = lineArray[8];
            ImageAltText = lineArray[9];
            ImageSource = lineArray[10];
            VideoUrl = lineArray[11];
            Notes = lineArray[12];
            _ = DateTime.TryParse(lineArray[13], out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[14], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[15]);
            TopicList = GetTagsFromString(lineArray[16]);
            TagList = GetTagsFromString(lineArray[17]);
            Tag2List = GetTagsFromString(lineArray[18]);
            Tag3List = GetTagsFromString(lineArray[19]);
            Tag4List = GetTagsFromString(lineArray[20]);
            LinkList = GetLinksFromString(lineArray[21]);
            _ = int.TryParse(lineArray[22], out var order);
            Order = order;
            ReviewEmail = lineArray[23];
            _ = DateTime.TryParse(lineArray[24], out var lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            FirstName,
            LastName,
            JobTitle,
            JobLocation,
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