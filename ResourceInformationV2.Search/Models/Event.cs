namespace ResourceInformationV2.Search.Models {

    public class Event : BaseObject {
        public override string EditLink => _editLink + "event/" + Id;
        public DateTime EndDate { get; set; }
        public bool IsAllDay { get; set; }
        public string Location { get; set; } = "";
        public override string NameType => "Event";
        public string Speaker { get; set; } = "";
        public string Sponsor { get; set; } = "";
        public DateTime StartDate { get; set; }
    }
}