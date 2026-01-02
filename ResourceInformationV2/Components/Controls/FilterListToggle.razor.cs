using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Controls {

    public partial class FilterListToggle {

        [Parameter]
        public List<string> AudienceEnabledTags { get; set; } = [];

        public IEnumerable<Tag>? AudienceTags => FilterTags?.Where(f => f.Key == TagType.Audience).SelectMany(x => x);
        public string AudienceTitle { get; set; } = "";

        [Parameter]
        public List<string> DepartmentEnabledTags { get; set; } = [];

        public IEnumerable<Tag>? DepartmentTags => FilterTags?.Where(f => f.Key == TagType.Department).SelectMany(x => x);
        public string DepartmentTitle { get; set; } = "";

        public IEnumerable<IGrouping<TagType, Tag>> FilterTags { get; set; } = [];

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public bool NoFiltersAvailable => FilterTags == null || FilterTags.Count() == 0;

        [Parameter]
        public List<string> Tag1EnabledTags { get; set; } = [];

        public string Tag1Title { get; set; } = "";

        [Parameter]
        public List<string> Tag2EnabledTags { get; set; } = [];

        public string Tag2Title { get; set; } = "";

        [Parameter]
        public List<string> Tag3EnabledTags { get; set; } = [];

        public string Tag3Title { get; set; } = "";

        [Parameter]
        public List<string> Tag4EnabledTags { get; set; } = [];

        public string Tag4Title { get; set; } = "";
        public IEnumerable<Tag>? Tags1 => FilterTags?.Where(f => f.Key == TagType.Tag1).SelectMany(x => x);
        public IEnumerable<Tag>? Tags2 => FilterTags?.Where(f => f.Key == TagType.Tag2).SelectMany(x => x);
        public IEnumerable<Tag>? Tags3 => FilterTags?.Where(f => f.Key == TagType.Tag3).SelectMany(x => x);
        public IEnumerable<Tag>? Tags4 => FilterTags?.Where(f => f.Key == TagType.Tag4).SelectMany(x => x);

        [Parameter]
        public List<string> TopicEnabledTags { get; set; } = [];

        public IEnumerable<Tag>? TopicTags => FilterTags?.Where(f => f.Key == TagType.Topic).SelectMany(x => x);
        public string TopicTitle { get; set; } = "";

        [Inject]
        protected FilterHelper FilterHelper { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            DepartmentEnabledTags ??= [];
            string sourceCode = await Layout.CheckSource();
            FilterTags = await FilterHelper.GetAllFilters(sourceCode);
            foreach (Tag? tag in FilterTags.SelectMany(x => x)) {
                if ((AudienceEnabledTags.Contains(tag.Title) && tag.TagType == TagType.Audience) ||
                    (DepartmentEnabledTags.Contains(tag.Title) && tag.TagType == TagType.Department) ||
                    (TopicEnabledTags.Contains(tag.Title) && tag.TagType == TagType.Topic) ||
                    (Tag1EnabledTags.Contains(tag.Title) && tag.TagType == TagType.Tag1) ||
                    (Tag2EnabledTags.Contains(tag.Title) && tag.TagType == TagType.Tag2) ||
                    (Tag3EnabledTags.Contains(tag.Title) && tag.TagType == TagType.Tag3) ||
                    (Tag4EnabledTags.Contains(tag.Title) && tag.TagType == TagType.Tag4)) {
                    tag.EnabledBySource = true;
                }
            }
            AudienceTitle = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Audience);
            DepartmentTitle = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Department);
            TopicTitle = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Topic);
            Tag1Title = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Tag1);
            Tag2Title = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Tag2);
            Tag3Title = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Tag3);
            Tag4Title = await SourceHelper.GetSourceFilterName(sourceCode, TagType.Tag4);
        }
    }
}