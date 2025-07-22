using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Publication {

    public partial class RelatedLinks {
        private LinkList _linkList = default!;

        public Search.Models.Publication Item { get; set; } = new();

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

        public async Task Save() {
            Layout.RemoveDirty();
            Item.LinkList = _linkList.GetSavedLinks();
            _ = await PublicationSetter.SetItem(Item);
            await Layout.Log(CategoryType.Publication, FieldType.Links, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await PublicationGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.PublicationItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}