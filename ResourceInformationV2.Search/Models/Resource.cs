namespace ResourceInformationV2.Search.Models {

    public class Resource : BaseObject {
        public override string EditLink => EditLinkRoot + "resource/" + Id;
        public override string NameType => "Resource";

        internal override string[] Headings => ["Id", "Title", "Description", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

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
            _ = DateTime.TryParse(lineArray[10], out DateTime createdDate);
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
            LinkList = GetLinksFromString(lineArray[19]);
            _ = int.TryParse(lineArray[20], out var order);
            Order = order;
            ReviewEmail = lineArray[21];
            _ = DateTime.TryParse(lineArray[22], out DateTime lastUpdatedDate);
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
            LinkList == null ? "" : string.Join(";", LinkList.Select(link => link.ToString())),
            Order.ToString(),
            ReviewEmail,
            LastUpdated.ToString("g")
        ];
    }
}