using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;

namespace ResourceInformationV2.Components.Pages.Note {

    public partial class Technical {
        private bool _originalStatus;

        public Search.Models.NoteItem Item { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected NoteSetter NoteSetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public async Task Delete() {
            Layout.RemoveDirty();
            _ = await NoteSetter.DeleteItem(Item.Id);
            await Layout.Log(CategoryType.Note, FieldType.Technical, Item, "Deletion");
            NavigationManager.NavigateTo("/note/edit");
        }

        public async Task Save() {
            Layout.RemoveDirty();
            if (Item.IsNewerDraft) {
                _ = await NoteSetter.PublishDraftItem(Item);
                await Layout.Log(CategoryType.Note, FieldType.Technical, Item, "Publish Draft Item", EmailType.OnPublication);
                NavigationManager.NavigateTo("/note/edit");
            } else {
                if (_originalStatus && !Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Note, FieldType.Technical, Item, "Moved To Draft", EmailType.OnDraft);
                } else if (!_originalStatus && Item.IsActive) {
                    Item.CreatedOn = DateTime.Now;
                    await Layout.Log(CategoryType.Note, FieldType.Technical, Item, "Published", EmailType.OnPublication);
                } else {
                    await Layout.Log(CategoryType.Note, FieldType.Technical, Item);
                }
                _ = await NoteSetter.SetItem(Item);
                _originalStatus = Item.IsActive;
                await Layout.AddMessage(Item.NameType + " saved successfully.");
            }
        }

        protected override async Task OnInitializedAsync() {
            var sourceCode = await Layout.CheckSource();
            var id = await Layout.GetCachedId();
            Item = await NoteGetter.GetItem(id);
            _originalStatus = Item.IsActive;
            Layout.SetSidebar(SidebarEnum.NotesItem, Item.Title);
            await base.OnInitializedAsync();
        }
    }
}