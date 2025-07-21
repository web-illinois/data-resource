using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class General {

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public Search.Models.Resource ResourceItem { get; set; } = default!;
        public string SourceCode { get; set; } = "";

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await ResourceSetter.SetItem(ResourceItem);
            await Layout.SetCacheId(ResourceItem.Id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, ResourceItem.Title);
            await Layout.Log(CategoryType.Resource, FieldType.General, ResourceItem);
            await Layout.AddMessage("Resource saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            SourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();

            if (!string.IsNullOrWhiteSpace(id)) {
                ResourceItem = await ResourceGetter.GetItem(id);
                Layout.SetSidebar(SidebarEnum.ResourceItem, ResourceItem.Title);
            } else {
                ResourceItem = new Search.Models.Resource() {
                    Source = SourceCode,
                    IsActive = false
                };
                Layout.SetSidebar(SidebarEnum.ResourceItem, "New Resource", true);
            }
            await base.OnInitializedAsync();
        }
    }
}