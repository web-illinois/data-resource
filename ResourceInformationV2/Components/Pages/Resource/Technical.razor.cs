using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class Technical {
        public Search.Models.Resource Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected ResourceSetter ResourceSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await ResourceSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Resource, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/resources/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await ResourceSetter.SetItem(Item);
            await Layout.Log(CategoryType.Resource, FieldType.Technical, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await ResourceGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}