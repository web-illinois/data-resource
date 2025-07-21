namespace ResourceInformationV2.Search.Models {

    public class Resource : BaseObject {
        public override string EditLink => _editLink + "resource/" + Id;
    }
}