using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class Image {
        private ImageControl _imageProgramImage = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public Search.Models.Resource ResourceItem { get; set; } = new();

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
            if (_imageProgramImage != null) {
                _ = await _imageProgramImage.SaveFileToPermanent();
            }
            _ = await ResourceSetter.SetItem(ResourceItem);
            await Layout.Log(CategoryType.Resource, FieldType.Links, ResourceItem);
            await Layout.AddMessage("Resource saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            ResourceItem = await ResourceGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, ResourceItem.Title);
            await base.OnInitializedAsync();
        }
    }
}