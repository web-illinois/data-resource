namespace ResourceInformationV2.Search.Models {

    public class Event : BaseObject {
        public override string EditLink => _editLink + "event/" + Id;
    }
}