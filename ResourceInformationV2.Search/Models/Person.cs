namespace ResourceInformationV2.Search.Models {

    public class Person : BaseObject {
        public override string EditLink => _editLink + "person/" + Id;
        public string FirstName { get; set; } = "";
        public string JobLocation { get; set; } = "";
        public string JobTitle { get; set; } = "";
        public string LastName { get; set; } = "";
        public string NameReversed => LastName + ", " + FirstName;
        public override string NameType => "Person";
        public override string Title => FirstName + " " + LastName;
    }
}