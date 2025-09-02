using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Person {

    public partial class Technical {
        private bool _originalStatus;
        public Search.Models.Person Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PersonSetter PersonSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await PersonSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Person, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/person/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            if (Item.IsNewerDraft) {
                _ = await PersonSetter.PublishDraftItem(Item);
                await Layout.Log(CategoryType.Person, FieldType.Technical, Item, "Publish Draft Item", EmailType.OnPublication);
                NavigationManager.NavigateTo("/person/edit");
            } else {
                _ = await PersonSetter.SetItem(Item);
                if (_originalStatus && !Item.IsActive) {
                    await Layout.Log(CategoryType.Person, FieldType.Technical, Item, "Moved To Draft", EmailType.OnDraft);
                } else if (!_originalStatus && Item.IsActive) {
                    await Layout.Log(CategoryType.Person, FieldType.Technical, Item, "Published", EmailType.OnPublication);
                } else {
                    await Layout.Log(CategoryType.Person, FieldType.Technical, Item);
                }
                _originalStatus = Item.IsActive;
                await Layout.AddMessage(Item.NameType + " saved successfully.");
            }
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await PersonGetter.GetItem(id);
            _originalStatus = Item.IsActive;
            Layout.SetSidebar(SidebarEnum.PeopleItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}