namespace ResourceInformationV2.Search.Models {

    public class NoteItem : BaseObject {
        public string DetailText { get; set; } = "";
        public override string EditLink => _editLink + "note/" + Id;
        public override string NameType => "Note";

        internal override string[] Headings => ["Id", "Title", "Description", "Detail Text", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Title = lineArray[1];
            Description = lineArray[2];
            DetailText = lineArray[3];
            Fragment = lineArray[4];
            Url = lineArray[5];
            Image = lineArray[6];
            ImageAltText = lineArray[7];
            ImageSource = lineArray[8];
            VideoUrl = lineArray[9];
            Notes = lineArray[10];
            _ = DateTime.TryParse(lineArray[11], out DateTime createdDate);
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
            LinkList = GetLinksFromString(lineArray[20]);
            _ = int.TryParse(lineArray[21], out var order);
            Order = order;
            ReviewEmail = lineArray[22];
            _ = DateTime.TryParse(lineArray[23], out DateTime lastUpdatedDate);
            LastUpdated = lastUpdatedDate == default ? DateTime.Now : lastUpdatedDate;
            return true;
        }

        internal override string[] SaveToStringPrivate() => [
            Id,
            Title,
            Description,
            DetailText,
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
            LinkList == null ? "" : string.Join(";", LinkList.Select(link => link.ToString())),
            Order.ToString(),
            ReviewEmail,
            LastUpdated.ToString("g")
        ];
    }
}