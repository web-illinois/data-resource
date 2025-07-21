namespace ResourceInformationV2.Search.Models {

    public enum UrlTypes {
        People,
        Publications,
        Resources,
        Faqs,
        Notes,
        Events,
    }

    public static class ExtensionTypes {

        public static string ConvertToUrlString(this Enum e) => "rr2_" + e.ToString().ToLowerInvariant();
    }
}