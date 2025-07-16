namespace ResourceInformationV2.Search.Models {

    public class GenericItem {
        public string Id { get; set; } = "";

        public bool IsActive { get; set; }

        public int Order { get; set; }

        public string Title { get; set; } = "";
    }
}