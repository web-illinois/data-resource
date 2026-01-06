namespace ResourceInformationV2.Search.Models {

    public class FaqItem : BaseObject {
        public override string Description { get => SummaryAnswer; set => SummaryAnswer = value; }
        public string DetailAnswer { get; set; } = "";
        public override string EditLink => _editLink + "faq/" + Id;
        public override string NameType => "FAQ Item";
        public string Question { get; set; } = "";
        public string SummaryAnswer { get; set; } = "";
        public override string Title { get => Question; set => Question = value; }

        internal override string[] Headings => ["Id", "Question", "Summary Answer", "Detail Answer", "Fragment", "Url", "Image", "Image Alt Text", "Image Source", "Video Url", "Notes", "Created Date", "Is Active", "Audience List", "Department List", "Topic List", "Tag 1 List", "Tag 2 List", "Tag 3 List", "Tag 4 List", "Related Links", "Order", "Review Email", "Last Updated Date"];

        internal override bool LoadFromStringPrivate(string[] lineArray) {
            Id = lineArray[0];
            Question = lineArray[1];
            SummaryAnswer = lineArray[2];
            DetailAnswer = lineArray[3];
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
            Question,
            SummaryAnswer,
            DetailAnswer,
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