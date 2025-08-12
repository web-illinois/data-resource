using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Search.JsonThinModels;

namespace ResourceInformationV2.Data.DataHelpers {

    public class FilterTranslator(SourceHelper sourceHelper) {
        private readonly SourceHelper _sourceHelper = sourceHelper;

        public static StaticCode TranslateTags(string sourceCode, IEnumerable<Tag> tags, string title, string codeString, int orderBy) {
            if (!tags.Any()) {
                return new();
            }
            return new() {
                CodeString = codeString,
                Codes = [.. tags.OrderBy(t => t.Order).ThenBy(t => t.Title).Select(t => t.Title)],
                Title = title,
                OrderBy = orderBy
            };
        }

        public async Task<Dictionary<string, string>> GetTagTitles(string sourceCode) => new Dictionary<string, string> {
                { "Tag 1", await _sourceHelper.GetSourceFilterName(sourceCode, TagType.Tag1) },
                { "Tag 2", await _sourceHelper.GetSourceFilterName(sourceCode, TagType.Tag2) },
                { "Tag 3", await _sourceHelper.GetSourceFilterName(sourceCode, TagType.Tag4) },
                { "Topic", await _sourceHelper.GetSourceFilterName(sourceCode, TagType.Topic) },
                { "Audience", await _sourceHelper.GetSourceFilterName(sourceCode, TagType.Audience) }
            };
    }
}