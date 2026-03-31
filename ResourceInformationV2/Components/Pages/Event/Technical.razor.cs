using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Event {

    public partial class Technical {
        private bool _originalStatus;
        public Search.Models.Event Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected EventGetter EventGetter { get; set; } = default!;

        [Inject]
        protected EventSetter EventSetter { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await EventSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Event, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/event/edit");
        }

        public async Task Save() {
            if (Item.IsNewerDraft) {
                _ = await EventSetter.PublishDraftItem(Item);
                await Layout.Log(CategoryType.Event, FieldType.Technical, Item, "Publish Draft Item", EmailType.OnPublication);
                NavigationManager.NavigateTo("/event/edit");
            } else {
                if (_originalStatus && !Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Event, FieldType.Technical, Item, "Moved To Draft", EmailType.OnDraft);
                } else if (!_originalStatus && Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Event, FieldType.Technical, Item, "Published", EmailType.OnPublication);
                } else {
                    await Layout.Log(CategoryType.Event, FieldType.Technical, Item);
                }
                _ = await EventSetter.SetItem(Item);
                _originalStatus = Item.IsActive;
                await Layout.AddMessage(Item.NameType + " saved successfully.");
            }
            Layout.RemoveDirty();
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await EventGetter.GetItem(id);
            _originalStatus = Item.IsActive;
            Layout.SetSidebar(SidebarEnum.EventItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}