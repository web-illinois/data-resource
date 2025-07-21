namespace ResourceInformationV2.Search.Models {

    public class NoteItem : BaseObject {
        public override string EditLink => _editLink + "note/" + Id;
    }
}