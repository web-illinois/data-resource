namespace ResourceInformationV2.Search.Models {

    public class GenericItem {
        public string EditLink { get; set; } = "";
        public string Id { get; set; } = "";

        public bool IsActive { get; set; }
        public bool IsNewerDraft { get; set; }
        public int Order { get; set; }

        public string Title { get; set; } = "";
    }
}