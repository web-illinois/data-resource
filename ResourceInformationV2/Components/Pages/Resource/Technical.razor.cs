using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Resource {

    public partial class Technical {
        private bool _originalStatus;
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
            NavigationManager.NavigateTo("/resource/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            if (Item.IsNewerDraft) {
                _ = await ResourceSetter.PublishDraftItem(Item);
                await Layout.Log(CategoryType.Resource, FieldType.Technical, Item, "Publish Draft Item", EmailType.OnPublication);
                NavigationManager.NavigateTo("/resource/edit");
            } else {
                if (_originalStatus && !Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Resource, FieldType.Technical, Item, "Moved To Draft", EmailType.OnDraft);
                } else if (!_originalStatus && Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Resource, FieldType.Technical, Item, "Published", EmailType.OnPublication);
                } else {
                    await Layout.Log(CategoryType.Resource, FieldType.Technical, Item);
                }
                _ = await ResourceSetter.SetItem(Item);
                _originalStatus = Item.IsActive;
                await Layout.AddMessage(Item.NameType + " saved successfully.");
            }
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await ResourceGetter.GetItem(id);
            _originalStatus = Item.IsActive;
            Layout.SetSidebar(SidebarEnum.ResourceItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}