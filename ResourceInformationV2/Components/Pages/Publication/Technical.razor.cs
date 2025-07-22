using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Publication {

    public partial class Technical {
        public Search.Models.Publication Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected PublicationGetter PublicationGetter { get; set; } = default!;

        [Inject]
        protected PublicationSetter PublicationSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await PublicationSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Resource, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/resources/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            _ = await PublicationSetter.SetItem(Item);
            await Layout.Log(CategoryType.Publication, FieldType.Technical, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await PublicationGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.PublicationItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}