namespace ResourceInformationV2.Search.Models {

    public class FaqItem : BaseObject {
        public override string Description { get => SummaryAnswer; set => SummaryAnswer = value; }
        public string DetailAnswer { get; set; } = "";
        public override string EditLink => _editLink + "faq/" + Id;
        public override string NameType => "FAQ Item";
        public string Question { get; set; } = "";
        public string SummaryAnswer { get; set; } = "";
        public override string Title { get => Question; set => Question = value; }
    }
}