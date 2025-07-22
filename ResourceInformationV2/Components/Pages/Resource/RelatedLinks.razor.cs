using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Controls;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class RelatedLinks {
        private LinkList _linkList = default!;

        public Search.Models.Resource Item { get; set; } = new();

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

        public async Task Save() {
            Layout.RemoveDirty();
            Item.LinkList = _linkList.GetSavedLinks();
            _ = await ResourceSetter.SetItem(Item);
            await Layout.Log(CategoryType.Resource, FieldType.Links, Item);
            await Layout.AddMessage(Item.NameType + " saved successfully.");
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            if (string.IsNullOrWhiteSpace(id)) {
                NavigationManager.NavigateTo("/");
            }
            Item = await ResourceGetter.GetItem(id);
            Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}