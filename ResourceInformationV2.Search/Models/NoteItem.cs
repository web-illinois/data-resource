namespace ResourceInformationV2.Search.Models {

    public class NoteItem : BaseObject {
        public string DetailText { get; set; } = "";
        public override string EditLink => EditLinkRoot + "note/" + Id;
        public override string NameType => "Note";

        internal override string[] Headings => ["Id", "Title", "Description", "Detail Text", "Fragment", "Link Url", "Link Text", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Title = PrepareForTextUpload(lineArray[1]);
            Description = PrepareForTextUpload(lineArray[2]);
            DetailText = PrepareForTextUpload(lineArray[3]);
            Fragment = lineArray[4];
            Url = lineArray[5];
            UrlText = lineArray[6];
            Image = lineArray[7];
            ImageAltText = lineArray[8];
            ImageSource = lineArray[9];
            VideoUrl = lineArray[10];
            Notes = PrepareForTextUpload(lineArray[11]);
            _ = DateTime.TryParse(lineArray[12], out var createdDate);
            CreatedOn = createdDate == default ? DateTime.Now : createdDate;
            _ = bool.TryParse(lineArray[13], out var isActive);
            IsActive = isActive;
            AudienceList = GetTagsFromString(lineArray[14]);
            DepartmentList = GetTagsFromString(lineArray[15]);
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
            Title,
            PrepareForTextDownload(Description),
            PrepareForTextDownload(DetailText),
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
            LinkList == null ? "" : string.Join(";", LinkList.Select(link => link.ToString())),
            Order.ToString(),
            ReviewEmail,
            LastUpdated.ToString("g")
        ];
    }
}