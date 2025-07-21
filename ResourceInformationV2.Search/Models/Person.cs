namespace ResourceInformationV2.Search.Models {

    public class Person : BaseObject {
        public override string EditLink => _editLink + "person/" + Id;
    }
}