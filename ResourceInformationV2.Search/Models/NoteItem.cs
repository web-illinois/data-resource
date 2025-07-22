namespace ResourceInformationV2.Search.Models {

    public class NoteItem : BaseObject {
        public string DetailText { get; set; } = "";
        public override string EditLink => _editLink + "note/" + Id;
        public override string NameType => "Note";
    }
}