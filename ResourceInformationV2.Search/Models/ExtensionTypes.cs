namespace ResourceInformationV2.Search.Models {

    public enum UrlTypes {
        People,
        Publications,
        Resources,
        Faq,
        Notes,
    }

    public static class ExtensionTypes {

        public static string ConvertToUrlString(this Enum e) => "rr2_" + e.ToString().ToLowerInvariant();
    }
}