using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Filters {

        private readonly Dictionary<string, TagType> _translator = new() {
            { "", TagType.None },
            { "Audience", TagType.Audience },
            { "Topic", TagType.Topic },
            { "Department", TagType.Department },
            { "Tag 1", TagType.Tag1 },
            { "Tag 2", TagType.Tag2 },
            { "Tag 3", TagType.Tag3 },
            { "Tag 4", TagType.Tag4 }
        };

        private int _sourceId;

        public List<Tag> FilterTags { get; set; } = [];
        public List<Tag> FilterTagsForDeletion { get; set; } = [];
        public string FilterTitle { get; set; } = "";
        public string FilterType { get; set; } = "";
        public TagType FilterTypeEnum => _translator[FilterType];

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string NewFilterName { get; set; } = "";
        public List<Tuple<string, string>> TagOrderList { get; set; } = [];

        [Inject]
        protected FilterHelper FilterHelper { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public void Add() {
            Layout.SetDirty();
            FilterTags.Add(new Tag { IsActive = true, LastUpdated = DateTime.Now, Title = NewFilterName, SourceId = _sourceId, TagType = FilterTypeEnum, Order = FilterTags.Count + 1 });
            NewFilterName = "";
        }

        public async Task ChangeFilter() {
            string sourceCode = await Layout.CheckSource();
            (FilterTags, _sourceId) = await FilterHelper.GetFilters(sourceCode, FilterTypeEnum);
            FilterTitle = await SourceHelper.GetSourceFilterName(sourceCode, FilterTypeEnum);
            FilterTagsForDeletion.Clear();
        }

        public void MoveDown(Tag tag) {
            Layout.SetDirty();
            FilterTags.MoveItemDown(tag);
        }

        public void MoveGroupDown(Tuple<string, string> s) {
            Layout.SetDirty();
            TagOrderList.MoveItemDown(s);
        }

        public void MoveGroupUp(Tuple<string, string> s) {
            Layout.SetDirty();
            TagOrderList.MoveItemUp(s);
        }

        public void MoveUp(Tag tag) {
            Layout.SetDirty();
            FilterTags.MoveItemUp(tag);
        }

        public void Remove(Tag tag) {
            Layout.SetDirty();
            FilterTags.Remove(tag);
            FilterTagsForDeletion.Add(tag);
        }

        public async Task<bool> Save() {
            if (string.IsNullOrWhiteSpace(FilterTitle) && FilterTags.Count > 0) {
                await Layout.AddMessage("You must enter a filter title before adding filters");
                return false;
            }
            string sourceCode = await Layout.CheckSource();
            await FilterHelper.SaveFilters(FilterTags, FilterTagsForDeletion, sourceCode);
            await SourceHelper.SetSourceFilterName(sourceCode, FilterTypeEnum, FilterTitle);
            await Layout.AddMessage($"Filters for {FilterType} have been saved");
            await OrderTagGroup(sourceCode);
            Layout.RemoveDirty();
            FilterTagsForDeletion.Clear();
            return true;
        }

        public async Task<bool> SaveGroupOrder() {
            string sourceCode = await Layout.CheckSource();
            await SourceHelper.SetSourceFilterOrder(sourceCode, string.Join(';', TagOrderList.Select(t => t.Item1)));
            foreach (var tag in TagOrderList) {
                await SourceHelper.SetSourceFilterName(sourceCode, (TagType)Enum.Parse(typeof(TagType), tag.Item1, true), tag.Item2);
            }
            await Layout.AddMessage($"Filters order has been saved");
            Layout.RemoveDirty();
            FilterTagsForDeletion.Clear();
            return true;
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            await Layout.CheckSource();
            string sourceCode = await Layout.CheckSource();
            await OrderTagGroup(sourceCode);
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
        }

        private async Task OrderTagGroup(string sourceCode) {
            TagOrderList = [.. await FilterHelper.GetTagTitles(sourceCode)];
        }
    }
}