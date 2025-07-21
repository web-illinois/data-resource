using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class Filters {
        private FilterListToggle _filterListToggle = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public Search.Models.Resource ResourceItem { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        public async Task Save() {
            ResourceItem.AudienceList = _filterListToggle.AudienceTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            ResourceItem.TopicList = _filterListToggle.TopicTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            ResourceItem.TagList = _filterListToggle.Tags1?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Layout.RemoveDirty();
            await Layout.AddMessage("Resource saved successfully.");
            await Layout.Log(CategoryType.Resource, FieldType.Filters, ResourceItem);
            _ = await ResourceSetter.SetItem(ResourceItem);
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            ResourceItem = await ResourceGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, ResourceItem.Title);
        }
    }
}