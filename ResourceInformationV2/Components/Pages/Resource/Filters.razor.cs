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

        public Search.Models.Resource Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        public async Task Save() {
            Item.AudienceList = _filterListToggle.AudienceTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.DepartmentList = _filterListToggle.DepartmentTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TopicList = _filterListToggle.TopicTags?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.TagList = _filterListToggle.Tags1?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.Tag2List = _filterListToggle.Tags2?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.Tag3List = _filterListToggle.Tags3?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Item.Tag4List = _filterListToggle.Tags4?.Where(t => t.EnabledBySource).Select(t => t.Title).ToList() ?? [];
            Layout.RemoveDirty();
            await Layout.AddMessage(Item.NameType + " saved successfully.");
            await Layout.Log(CategoryType.Resource, FieldType.Filters, Item);
            _ = await ResourceSetter.SetItem(Item);
        }

        protected override async Task OnInitializedAsync() {
            string sourceCode = await Layout.CheckSource();
            string id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await ResourceGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
        }
    }
}